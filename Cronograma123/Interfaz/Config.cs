using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CronogramaMe.Interfaz
{
    class Config
    {
        public bool primeraEjecucion { get; set; } = true;
        public bool compruebaExcelDisponibleAlIniciar { get; set; } = true;
        public string tutorialUrl { get; set; } = @"www.youtube.com";
    }
}
