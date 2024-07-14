using AvaliadorSalas.Data;
using AvaliadorSalas.Models;
using AvaliadorSalas.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace AvaliadorSalas.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ArduinoDbContext _context;

        public HomeController(ILogger<HomeController> logger, ArduinoDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        /// <summary>
        /// Home page
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            var vm = new RelatorioVm { };
           
           vm.LastRead = MostRecent();
    

            return View(vm);
        }

        public IActionResult GraficoTemp()
        {
            var vm = new RelatorioTempVm { };

            vm.LastRead = MostRecentTemp();

            return View(vm);
        }

        public IActionResult GraficoHum()
        {
            var vm = new RelatorioHumVm { };

            vm.LastRead = MostRecentHum();

            return View(vm);
        }

        public IActionResult GraficoCo2()
        {
            var vm = new RelatorioCo2Vm { };

            vm.LastRead = MostRecentCo2();

            return View(vm);
        }

        /// <summary>
        /// Método que permite obter as últimas leituras realizados pelos sensores
        /// </summary>
        /// <returns></returns>
        public LeituraVm MostRecent()
        {
            var recente = new LeituraVm();

            var last3 = _context.Medicoes.
                OrderByDescending(m => m.DataMedicao).Take(3).ToList();

            if (last3.Any())
            {
                var temp = last3.FirstOrDefault(m => m.SensorId == 1);
                var humd = last3.FirstOrDefault(m => m.SensorId == 2);
                var co2 = last3.FirstOrDefault(m => m.SensorId == 3);

                if (temp != null)
                {
                    recente.DataMedicao = temp.DataMedicao;
                    recente.Temperatura = temp.Medicao;
                }

                if (humd != null) { recente.Humidade = humd.Medicao; }
                if (co2 != null) { recente.Co2 = co2.Medicao; }
            }

            return recente;
        }

        /// <summary>
        /// Método que permite obter as últimas leituras realizados pelos sensores
        /// </summary>
        /// <returns></returns>
        public LeituraTempVm MostRecentTemp()
        {
            var recente = new LeituraTempVm();

            var last1 = _context.Medicoes.
                OrderByDescending(m => m.DataMedicao).Take(1).ToList();

            if (last1.Any())
            {
                var temp = last1.FirstOrDefault(m => m.SensorId == 1);


                if (temp != null)
                {
                    recente.DataMedicao = temp.DataMedicao;
                    recente.Temperatura = temp.Medicao;
                }

                
            }

            return recente;
        }

        /// <summary>
        /// Método que permite obter as últimas leituras realizados pelos sensores
        /// </summary>
        /// <returns></returns>
        public LeituraHumVm MostRecentHum()
        {
            var recente = new LeituraHumVm();

            var last1 = _context.Medicoes.
                OrderByDescending(m => m.DataMedicao).Take(1).ToList();

            if (last1.Any())
            {
                var hum = last1.FirstOrDefault(m => m.SensorId == 2);


                if (hum != null)
                {
                    recente.DataMedicao = hum.DataMedicao;
                    recente.Humidade = hum.Medicao;
                }


            }

            return recente;
        }

        /// <summary>
        /// Método que permite obter as últimas leituras realizados pelos sensores
        /// </summary>
        /// <returns></returns>
        public LeituraCo2Vm MostRecentCo2()
        {
            var recente = new LeituraCo2Vm();

            var last1 = _context.Medicoes.
                OrderByDescending(m => m.DataMedicao).Take(1).ToList();

            if (last1.Any())
            {
                var co2 = last1.FirstOrDefault(m => m.SensorId == 3);


                if (co2 != null)
                {
                    recente.DataMedicao = co2.DataMedicao;
                    recente.Co2 = co2.Medicao;
                }


            }

            return recente;
        }

        /// <summary>
        /// Método que permite obter todas as leituras realizados pelos sensores
        /// </summary>
        /// <returns></returns>
        public IActionResult Todas()
        {
            List<Leitura> recente = new List<Leitura>();

            recente = _context.Medicoes.
                Include(t => t.Sensor).
                OrderByDescending(m => m.DataMedicao).ToList();

            return View(recente);
        }

        /// <summary>
        /// Método que recebe os 3 valores do dispositivo e grava na base de dados
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="humidity"></param>
        /// <param name="co2"></param>
        /// <returns></returns>
        public ActionResult SaveReadings(decimal? temp, decimal? humidity, decimal? co2, string sala)
        {
            var results = "Sucesso";
            var reported = DateTime.Now;
            try
            {
                    if (temp.HasValue)
                    {
                        //  temperatura
                        _context.Medicoes.Add(new Leitura
                        {
                            SensorId = (int)TipoLeituraEnum.Temperatura,
                            Medicao = temp.Value,
                            DataMedicao = reported,
                            Sala = sala
                        });
                    }
                    if (humidity.HasValue)
                    {
                        //  humidade
                        _context.Medicoes.Add(new Leitura
                        {
                            SensorId = (int)TipoLeituraEnum.Humidade,
                            Medicao = humidity.Value,
                            DataMedicao = reported,
                            Sala = sala
                        });
                    }
                    if (co2.HasValue)
                    {
                        //  co2
                        _context.Medicoes.Add(new Leitura
                        {
                            SensorId = (int)TipoLeituraEnum.Co2,
                            Medicao = co2.Value,
                            DataMedicao = reported,
                            Sala = sala
                        });
                    }
                    // grava na base de dados
                    _context.SaveChanges();
                
            }
            catch (Exception ex)
            {
                results = "Erro: " + ex.Message;
            }

            return Content(results);
        }


        /// <summary>
        /// Método que permite obter as leituras realizados pelos sensores nas 24 horas mais recentes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ReadingDayTemp()
        {
            // establish an empty table
            var gdataTable = new GoogleVizDataTable();

                // next get the most recent measurement for this device
                var mostRecent = _context.Medicoes
                    .Select(m => m).OrderByDescending(m => m.DataMedicao).Take(1).FirstOrDefault();

                // if we have a recent measurement for this device
                if (mostRecent != null)
                {
                    // establish a range of previous to current day/time
                    var finish = mostRecent.DataMedicao;
                    var start = finish.AddDays(-1);

                    // fetch a set of measurements for that range
                    var recentSet = MedicaoSetRangeTemp(start, finish);

                return Json(recentSet);
            }

            return NotFound();
        }

        /// <summary>
        /// Método que permite obter as leituras realizados pelos sensores nas 24 horas mais recentes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ReadingDayHum()
        {
            // establish an empty table
            var gdataTable = new GoogleVizDataTable();

            // next get the most recent measurement for this device
            var mostRecent = _context.Medicoes
                .Select(m => m).OrderByDescending(m => m.DataMedicao).Take(1).FirstOrDefault();

            // if we have a recent measurement for this device
            if (mostRecent != null)
            {
                // establish a range of previous to current day/time
                var finish = mostRecent.DataMedicao;
                var start = finish.AddDays(-1);

                // fetch a set of measurements for that range
                var recentSet = MedicaoSetRangeHum(start, finish);

                return Json(recentSet);
            }

            return NotFound();
        }

        /// <summary>
        /// Método que permite obter as leituras realizados pelos sensores nas 24 horas mais recentes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult ReadingDayCo2()
        {
            // establish an empty table
            var gdataTable = new GoogleVizDataTable();

            // next get the most recent measurement for this device
            var mostRecent = _context.Medicoes
                .Select(m => m).OrderByDescending(m => m.DataMedicao).Take(1).FirstOrDefault();

            // if we have a recent measurement for this device
            if (mostRecent != null)
            {
                // establish a range of previous to current day/time
                var finish = mostRecent.DataMedicao;
                var start = finish.AddDays(-1);

                // fetch a set of measurements for that range
                var recentSet = MedicaoSetRangeCo2(start, finish);

                return Json(recentSet);
            }

            return NotFound();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">início do intervalo de pesquisa</param>
        /// <param name="finish">fim do intervalo de pesquisa</param>
        /// <returns></returns>
        public List<LeituraTempVm> MedicaoSetRangeTemp(DateTime start, DateTime finish)
        {
            // constrói o conjunto de medições
            var measureSet =
                (from m in _context.Medicoes.Select(m => m).AsEnumerable()
                 where m.DataMedicao >= start && m.DataMedicao <= finish
                 orderby m.DataMedicao
                 group m by new { MeasuredDate = DateTime.Parse(m.DataMedicao.ToString("yyyy-MM-dd HH:mm:ss"))}
                    into g
                 select new LeituraTempVm
                 {
                     DataMedicao = g.Key.MeasuredDate,
                     Temperatura = g.Where(m => m.SensorId == 1).Select(r => r.Medicao).FirstOrDefault()
                 }).ToList();

            return measureSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">início do intervalo de pesquisa</param>
        /// <param name="finish">fim do intervalo de pesquisa</param>
        /// <returns></returns>
        public List<LeituraHumVm> MedicaoSetRangeHum(DateTime start, DateTime finish)
        {
            // constrói o conjunto de medições
            var measureSet =
                (from m in _context.Medicoes.Select(m => m).AsEnumerable()
                 where m.DataMedicao >= start && m.DataMedicao <= finish
                 orderby m.DataMedicao
                 group m by new { MeasuredDate = DateTime.Parse(m.DataMedicao.ToString("yyyy-MM-dd HH:mm:ss")) }
                    into g
                 select new LeituraHumVm
                 {
                     DataMedicao = g.Key.MeasuredDate,
                     Humidade = g.Where(m => m.SensorId == 2).Select(r => r.Medicao).FirstOrDefault()
                 }).ToList();

            return measureSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start">início do intervalo de pesquisa</param>
        /// <param name="finish">fim do intervalo de pesquisa</param>
        /// <returns></returns>
        public List<LeituraCo2Vm> MedicaoSetRangeCo2(DateTime start, DateTime finish)
        {
            // constrói o conjunto de medições
            var measureSet =
                (from m in _context.Medicoes.Select(m => m).AsEnumerable()
                 where m.DataMedicao >= start && m.DataMedicao <= finish
                 orderby m.DataMedicao
                 group m by new { MeasuredDate = DateTime.Parse(m.DataMedicao.ToString("yyyy-MM-dd HH:mm:ss")) }
                    into g
                 select new LeituraCo2Vm
                 {
                     DataMedicao = g.Key.MeasuredDate,
                     Co2 = g.Where(m => m.SensorId == 3).Select(r => r.Medicao).FirstOrDefault()
                 }).ToList();

            return measureSet;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
