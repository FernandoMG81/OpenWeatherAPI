using DB.Context;
using DB.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OpenWeatherAPI.Models;
using System;
using System.Net.Http;

using System.Threading.Tasks;

namespace OpenWeatherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly WeatherContext _context;
        private readonly HttpClient _httpClient;

        public WeatherController(WeatherContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _httpClient = httpClientFactory.CreateClient();
        }

        /// <summary>
        /// Agrega datos meteorológicos para una ciudad específica.
        /// </summary>
        /// <param name="cityData">Datos de la ciudad, incluyendo su identificador y timestamp.</param>
        /// <returns>Resultado de la operación. OK si la operación se realizó con éxito.</returns>
        [HttpPost("add")]
        public async Task<ActionResult<string>> AddWeatherData([FromBody] CityModel cityData)
        {
            try
            {
                var apiKey = Properties.Resources.OPEN_WEATHER_API_KEY;
                var uri = "http://api.openweathermap.org";
                var apiUrl = $"/data/2.5/weather?id={cityData.CityID}&units=metric&appid={apiKey}";

                var response = await _httpClient.GetAsync($"{uri}{apiUrl}");
                response.EnsureSuccessStatusCode();

                var stringResult = await response.Content.ReadAsStringAsync();
                var weatherData = JsonConvert.DeserializeObject<OpenWeatherMapResponse>(stringResult);


                var existingCity = _context.Cities.FirstOrDefault(c => c.CityID == cityData.CityID);
             
                if (existingCity == null)
                {
                    var newCity = new City
                    {
                        CityID = weatherData.Id,
                        Name = weatherData.Name,
                        CountryCode = weatherData.Sys.Country,
                    };
                    _context.Cities.Add(newCity);
                    _context.SaveChanges();

                    existingCity = newCity;
                }

                var newHistoricalData = new WeatherRecord
                {
                    CityID = existingCity.CityID,
                    Timestamp = cityData.Timestamp,
                    Temp = weatherData.Main.Temp,
                    Humidity = weatherData.Main.Humidity,
                    FeelsLike = weatherData.Main.Feels_Like,
                    Icon = weatherData.Weather[0].Icon,
                };

                _context.WeatherRecords.Add(newHistoricalData);
                _context.SaveChanges();

                return Ok("Weather data added successfully.");
            }
            catch (HttpRequestException httpRequestException)
            {
                return BadRequest($"Error getting weather from OpenWeather: {httpRequestException.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el historial completo de registros meteorológicos para una ciudad específica.
        /// </summary>
        /// <param name="cityId">Identificador único de la ciudad para la cual se desea obtener el historial meteorológico.</param>
        /// <returns>ActionResult con el historial meteorológico de la ciudad.</returns>
        [HttpGet("allrecords/{cityId}")]
        public ActionResult<IEnumerable<DB.Entities.WeatherRecord>> GetFullHistory(int cityId)
        {
            try
            {
                var history = _context.WeatherRecords
                    .Include(h => h.City)
                    .Where(h => h.CityID == cityId)
                    .OrderByDescending(h => h.Timestamp)
                    .ToList();

                return Ok(history);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Error interno del servidor");
            }
        }


        /// <summary>
        /// Obtiene el registro meteorológico más reciente para una ciudad específica.
        /// </summary>
        /// <param name="cityId">Identificador único de la ciudad para la cual se desea obtener el registro meteorológico más reciente.</param>
        /// <returns>ActionResult con el registro meteorológico más reciente de la ciudad.</returns>
        [HttpGet("latest/{cityId}")]
        public ActionResult<WeatherRecord> GetLatest(int cityId)
        {
            try
            {
                var latestEntry = _context.WeatherRecords
                    .Include(h => h.City)
                    .Where(h => h.CityID == cityId)
                    .OrderByDescending(h => h.Timestamp)
                    .FirstOrDefault();

                return Ok(latestEntry);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Error interno del servidor");
            }
        }


        /// <summary>
        /// Elimina un registro meteorológico específico para una ciudad y marca de tiempo dadas.
        /// </summary>
        /// <param name="deleteRecordModel">Datos del modelo para eliminar el registro, incluyendo el identificador de la ciudad y timestamp.</param>
        /// <returns>ActionResult con el resultado de la operación de eliminación.</returns>
        [HttpDelete("delete")]
        public ActionResult<string> DeleteRecord([FromBody] CityModel deleteRecordModel)
        {
            try
            {
                var recordToDelete = _context.WeatherRecords
                    .FirstOrDefault(r => r.CityID == deleteRecordModel.CityID && r.Timestamp == deleteRecordModel.Timestamp);

                if (recordToDelete == null)
                {
                    return NotFound($"Weather record with ID {deleteRecordModel.CityID} and timestamp {deleteRecordModel.Timestamp} not found.");
                }

                _context.WeatherRecords.Remove(recordToDelete);
                _context.SaveChanges();

                return Ok($"Weather record with ID {deleteRecordModel.CityID} and timestamp {deleteRecordModel.Timestamp} deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}


