using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using ThaiNationalIDCard.NET.Models;

namespace ThaiNationalIDCard.NET.Example.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadersController : ControllerBase
    {
        private readonly ThaiNationalIDCardReader cardReader;

        public ReadersController(ThaiNationalIDCardReader cardReader)
        {
            this.cardReader = cardReader;
        }

        [HttpGet("Read")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PersonalPhoto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Read()
        {
            try
            {
                PersonalPhoto personalPhoto = cardReader.GetPersonalPhoto();

                return Ok(personalPhoto);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
    }
}