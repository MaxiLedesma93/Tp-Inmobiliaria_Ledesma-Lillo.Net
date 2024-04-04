using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Models
{
    public class Inmueble 
   {
		[Key]
        [Display(Name = "Nº")]
		public int IdInmueble { get; set; }
		[Required]
		[Display(Name = "Dirección")]
		public string? Direccion { get; set; }
		[Required]
		public int Ambientes { get; set; }
		[Required]
		public int Superficie { get; set; }
		public decimal Latitud { get; set; }

        public string? Uso {get; set; }
		public decimal Longitud { get; set; }
		[Display(Name = "Dueño"), Required]
		
		//clave [ForeignKey(nameof(PropietarioId)), Required]
		public int PropietarioId { get; set; }
		
		
		public Propietario? Duenio { get; set; }
   }
   
}