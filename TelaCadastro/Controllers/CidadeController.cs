﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using DAL.Model;
using DAL.Persistence;
using TelaCadastro.Models;
using TelaCadastro.Util;
using TelaCadastro.ViewModels;

namespace TelaCadastro.Controllers
{
    public class CidadeController : Controller
    {
        public ActionResult Index()
        {
            var listaCidade = new CidadeDal().ObterTodos().ToList();
            var quantidade = listaCidade.Count();

            var paginacao = 5;

            var grid = new TabelaGenerica<Cidade>
            {
                Dados = listaCidade.ToList<Cidade>().OrderBy(ent => ent.nome).
                Take(paginacao > quantidade ? quantidade : paginacao).ToList(),
                Paginacao = paginacao,
                ClassesCss = "table table-hover",
                PaginaAtual = 1,
                TotalRegistros = quantidade
            };

            return View(grid);
        }

        public ActionResult TabelaCidade(Filtro[] filtros)
        {
            var paginacao = 5;
            var paginaAtual = 1;

            var listacidade = new CidadeDal().ObterTodos().ToList();

            if (filtros != null)
            {
                var listaFiltrosValidos = filtros.ToList<Filtro>().Where(ent => !String.IsNullOrEmpty(ent.value));
                foreach (var filtro in listaFiltrosValidos)
                {
                    switch (filtro.name)
                    {
                        case "codigo":
                            var id = Convert.ToInt32(filtro.value);
                            listacidade = listacidade.Where(ent => ent.cidadeid == id).ToList();
                            break;
                        case "nome":
                            listacidade = listacidade.Where(ent => ent.nome.Contains(filtro.value)).ToList();
                            break;
                        case "cep":
                            var cep = RemoveMascara(filtro.value);
                            listacidade = listacidade.Where(ent => ent.cep.Contains(cep)).ToList();
                            break;
                        case "estado":
                            listacidade = listacidade.Where(ent => ent.estado.Contains(filtro.value)).ToList();
                            break;
                        case "Paginacao":
                            paginacao = StrToInt32(filtro.value);
                            break;
                        case "PaginaAtual":
                            paginaAtual = StrToInt32(filtro.value);
                            break;
                    }
                }
            }

            int quantidade = listacidade.Count();

            var grid = new TabelaGenerica<Cidade>
            {
                Dados = listacidade.OrderBy(ent => ent.nome)
              .ToList()
              .Skip(paginacao * (paginaAtual - 1)).Take(paginacao > quantidade ? quantidade : paginacao).ToList(),
                Paginacao = paginacao,
                ClassesCss = "table table-hover",
                PaginaAtual = paginaAtual,
                TotalRegistros = quantidade
            };

            return View(grid);
        }

        public ActionResult Ordenar(string campo, string ordem)
        {

            var listacidade = new CidadeDal().ObterTodos().ToList();

            if (ordem == "cres")
            {
                switch (campo)
                {
                    case "cidadeid":
                        listacidade = listacidade.OrderBy(ent => ent.cidadeid).ToList();
                        break;
                    case "nome":
                        listacidade = listacidade.OrderBy(ent => ent.nome).ToList();
                        break;
                    case "estado":
                        listacidade = listacidade.OrderBy(ent => ent.estado).ToList();
                        break;
                    case "cep":
                        listacidade = listacidade.OrderBy(ent => ent.cep).ToList();
                        break;
                }

            }
            else
            {
                switch (campo)
                {
                    case "cidadeid":
                        listacidade = listacidade.OrderByDescending(ent => ent.cidadeid).ToList();
                        break;
                    case "nome":
                        listacidade = listacidade.OrderByDescending(ent => ent.nome).ToList();
                        break;
                    case "estado":
                        listacidade = listacidade.OrderByDescending(ent => ent.estado).ToList();
                        break;
                    case "cep":
                        listacidade = listacidade.OrderByDescending(ent => ent.cep).ToList();
                        break;
                }
            }

            var paginacao = StrToInt32(ConfigurationManager.AppSettings["PaginacaoPadrao"]);
            var paginaAtual = 1;

            int quantidade = listacidade.Count();

            var grid = new TabelaGenerica<Cidade>
            {
                Dados = listacidade.ToList()
              .Skip(paginacao * (paginaAtual - 1)).Take(paginacao > quantidade ? quantidade : paginacao).ToList(),
                Paginacao = paginacao,
                PaginaAtual = paginaAtual,
                TotalRegistros = quantidade
            };

            return View("TabelaCidade", grid);
        }

        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Incluir(CidadeViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                CidadeDal serviceCidade = new CidadeDal();

                Cidade model = new Cidade
                {
                    nome = viewmodel.nome,
                    estado = viewmodel.estado,
                    cep = RemoveMascara(viewmodel.cep)
                };

                serviceCidade.Incluir(model);

                return RedirectToAction("Index", "Cidade");
            }
            else
            {
                return View(viewmodel);
            }
        }

        public ActionResult Alterar(int id)
        {
            var obj = new CidadeDal().Obter(id);

            var viewmodel = new CidadeViewModel
            {
                nome = obj.nome,
                estado = obj.estado,
                cep = obj.cep,
                cidadeid = obj.cidadeid
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Alterar(CidadeViewModel viewmodel)
        {
            if (ModelState.IsValid)
            {
                CidadeDal serviceCidade = new CidadeDal();

                var cidade = serviceCidade.Obter(viewmodel.cidadeid);

                cidade.nome = viewmodel.nome;
                cidade.estado = viewmodel.estado;
                cidade.cep = RemoveMascara(viewmodel.cep);

                serviceCidade.Alterar(cidade);

                return RedirectToAction("Index", "Cidade");
            }
            else
            {
                return View(viewmodel);
            }
        }

        public ActionResult Visualizar(int id)
        {
            var obj = new CidadeDal().Obter(id);

            var viewmodel = new CidadeViewModel
            {
                nome = obj.nome,
                estado = obj.estado,
                cep = obj.cep,
                cidadeid = obj.cidadeid
            };

            return View(viewmodel);
        }

        public ActionResult Excluir(int id)
        {
            var obj = new CidadeDal().Obter(id);

            var viewmodel = new CidadeViewModel
            {
                nome = obj.nome,
                estado = obj.estado,
                cep = obj.cep,
                cidadeid = obj.cidadeid
            };

            return View(viewmodel);
        }

        [HttpPost]
        public ActionResult Excluir(CidadeViewModel viewmodel)
        {
            try
            {
                CidadeDal serviceCidade = new CidadeDal();
                serviceCidade.Excluir(ent => ent.cidadeid == viewmodel.cidadeid);

                return RedirectToAction("Index", "Cidade");

            }
            catch (Exception ex)
            {
                return View();
            }

        }

        public int StrToInt32(string valor)
        {
            if (String.IsNullOrWhiteSpace(valor))
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(valor);
            }
        }

        public string RemoveMascara(string texto)
        {
            return texto == null ? null : (Regex.Replace(texto, "[?\\)?\\(_./-]", "")).Replace(" ", "");
        }
    }
}