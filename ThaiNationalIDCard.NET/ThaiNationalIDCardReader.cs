using PCSC;
using PCSC.Exceptions;
using PCSC.Iso7816;
using PCSC.Utils;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using ThaiNationalIDCard.NET.Interfaces;
using ThaiNationalIDCard.NET.Models;

namespace ThaiNationalIDCard.NET
{
    public class ThaiNationalIDCardReader
    {
        private readonly ISCardContext context;
        private readonly ISCardReader reader;

        private SCardError error;
        private IntPtr intPtr;
        private IThaiNationalIDCardAPDUCommand apdu;

        public ThaiNationalIDCardReader()
        {
            context = ContextFactory.Instance.Establish(SCardScope.System);
            reader = new SCardReader(context);
            apdu = new ThaiNationalIDCardAPDUCommandType02();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public Personal GetPersonal()
        {
            try
            {
                Open();

                return GetPersonalInfo();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                Close();
            }
        }

        public PersonalPhoto GetPersonalPhoto()
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                Open();

                Personal personal = GetPersonalInfo();
                PersonalPhoto personalPhoto = new PersonalPhoto(personal);

                byte[][] photoCommand = apdu.PhotoCommand;
                byte[] responseBuffer;

                for (int i = 0; i < photoCommand.Length; i++)
                {
                    responseBuffer = SendCommand(photoCommand[i]);
                    stream.Write(responseBuffer, 0, responseBuffer.Length);
                }

                stream.Seek(0, SeekOrigin.Begin);

                personalPhoto.Photo = string.Format($"data:image/jpeg;base64,{Convert.ToBase64String(stream.ToArray())}");

                return personalPhoto;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                stream.Dispose();

                Close();
            }
        }

        private Personal GetPersonalInfo()
        {
            try
            {
                Personal personal = new Personal();

                personal.CitizenID = GetUTF8FromAsciiBytes(SendCommand(apdu.CitizenIDCommand));

                string personalInfo = GetUTF8FromAsciiBytes(SendCommand(apdu.PersonalInfoCommand));
                string thaiPersonalInfo = personalInfo.Substring(0, 100);
                string englishPersonalInfo = personalInfo.Substring(100, 100);

                personal.ThaiPersonalInfo = new PersonalInfo(thaiPersonalInfo);
                personal.EnglishPersonalInfo = new PersonalInfo(englishPersonalInfo);
                personal.Sex = personalInfo.Substring(208, 1);

                string addressInfo = GetUTF8FromAsciiBytes(SendCommand(apdu.AddressInfoCommand));

                personal.AddressInfo = new AddressInfo(addressInfo);

                string cardIssueExpire = GetUTF8FromAsciiBytes(SendCommand(apdu.CardIssueExpireCommand));

                personal.IssueDate = new DateTime(Convert.ToInt32(cardIssueExpire.Substring(0, 4)) - 543
                    , Convert.ToInt32(cardIssueExpire.Substring(4, 2))
                    , Convert.ToInt32(cardIssueExpire.Substring(6, 2))
                );
                personal.ExpireDate = new DateTime(Convert.ToInt32(cardIssueExpire.Substring(8, 4)) - 543
                    , Convert.ToInt32(cardIssueExpire.Substring(12, 2))
                    , Convert.ToInt32(cardIssueExpire.Substring(14, 2))
                );
                personal.Issuer = GetUTF8FromAsciiBytes(SendCommand(apdu.CardIssuerCommand));

                return personal;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private string GetUTF8FromAsciiBytes(byte[] asciiBytes)
        {
            byte[] utf8 = Encoding.Convert(
                Encoding.GetEncoding("TIS-620"),
                Encoding.UTF8,
                asciiBytes
                );

            return Encoding.UTF8.GetString(utf8);
        }

        private byte[] SendCommand(byte[] command)
        {
            byte[] responseBuffer = new byte[256];

            error = reader.Transmit(intPtr, command, ref responseBuffer);
            HandleError();

            ResponseApdu responseApdu = new ResponseApdu(responseBuffer, IsoCase.Case2Short, reader.ActiveProtocol);

            if (responseApdu.SW1.Equals((byte)SW1Code.NormalDataResponse))
            {
                command = apdu.GetResponse().Concat(new byte[] { responseApdu.SW2 }).ToArray();
                responseBuffer = new byte[258];

                error = reader.Transmit(intPtr, command, ref responseBuffer);
                HandleError();

                if (responseBuffer.Length - responseApdu.SW2 == 2)
                {
                    return responseBuffer.Take(responseBuffer.Length - 2).ToArray();
                }
            }

            return responseBuffer;
        }

        private void Close()
        {
            reader.Disconnect(SCardReaderDisposition.Leave);
            context.Release();
        }

        private void Open()
        {
            try
            {
                Thread.Sleep(1500);

                string[] szReaders = context.GetReaders();
                if (szReaders.Length <= 0)
                    throw new PCSCException(SCardError.NoReadersAvailable, "Could not find any Smartcard reader.");

                error = reader.Connect(szReaders[0], SCardShareMode.Shared, SCardProtocol.T0 | SCardProtocol.T1);
                HandleError();

                intPtr = new IntPtr();
                switch (reader.ActiveProtocol)
                {
                    case SCardProtocol.T0:
                        intPtr = SCardPCI.T0;
                        break;
                    case SCardProtocol.T1:
                        intPtr = SCardPCI.T1;
                        break;
                    case SCardProtocol.Raw:
                        intPtr = SCardPCI.Raw;
                        break;
                    default:
                        throw new PCSCException(SCardError.ProtocolMismatch, "Protocol not supported: " + reader.ActiveProtocol.ToString());
                }

                error = reader.Status(out string[] readerNames, out SCardState state, out SCardProtocol protocol, out byte[] atrs);
                HandleError();

                if (atrs == null || atrs.Length < 2)
                    throw new PCSCException(SCardError.InvalidAtr);

                if (atrs[0] == 0x3B && atrs[1] == 0x67)
                    apdu = new ThaiNationalIDCardAPDUCommandType01();

                if (!SelectApplet())
                    throw new Exception("SmartCard not support (Can't select Ministry of Interior Applet).");
            }
            catch (PCSCException e)
            {
                throw e;
            }
        }

        private bool SelectApplet()
        {
            byte[] command = apdu.Select(apdu.MinistryOfInteriorAppletCommand);
            byte[] responseBuffer = new byte[256];

            error = reader.Transmit(intPtr, command, ref responseBuffer);
            HandleError();

            ResponseApdu responseApdu = new ResponseApdu(responseBuffer, IsoCase.Case2Short, reader.ActiveProtocol);

            return responseApdu.SW1.Equals((byte)SW1Code.NormalDataResponse) || responseApdu.SW1.Equals((byte)SW1Code.Normal);
        }

        private void HandleError()
        {
            if (error == SCardError.Success)
                return;

            throw new PCSCException(error, SCardHelper.StringifyError(error));
        }
    }
}
