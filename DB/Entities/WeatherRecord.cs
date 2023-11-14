using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB.Entities
{
    // Especifica que CityID y Timestamp forman una clave primaria compuesta.
    [PrimaryKey(nameof(CityID), nameof(Timestamp))]
    public class WeatherRecord
    {
        public int CityID { get; set; }
        [Column(TypeName = "bigint")]
        public long Timestamp { get; set; }
        public float Temp { get; set; }
        public int Humidity { get; set; }
        public float FeelsLike { get; set; }
        public string Icon { get; set; }

        // Define la relación con la entidad City mediante la clave foránea CityID.
        [ForeignKey("CityID")]
        public virtual City City { get; set; }
    }
}
