using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Patient;
using ClinAgenda.src.Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace ClinAgenda.src.WebAPI.Controllers
{
    public class PatientController : ControllerBase
    {
        private readonly PatientUseCase _patientUseCase;

        public PatientController(PatientUseCase service)
        {
            _patientUseCase = service;
        }
        [HttpGet("list")]
        public async Task<IActionResult> GetPatientsAsync([FromQuery] string? name, [FromQuery] string? documentNumber, [FromQuery] int? statusId, [FromQuery] int itemsPerPage = 10, [FromQuery] int page = 1)
        {
            try
            {
                var result = await _patientUseCase.GetPatientsAsync(name, documentNumber, statusId, itemsPerPage, page);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }

        [HttpPost("insert")]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientInsertDTO patient)
        {
            try
            {
                if (patient == null)
                {
                    return BadRequest("Dados inválidos para criação do paciente.");
                }

                var createdPatient = await _patientUseCase.CreatePatientAsync(patient);

                if (!(createdPatient > 0))
                {
                    return StatusCode(500, "Erro ao criar o paciente.");
                }

                var infosPatientCreated = await _patientUseCase.GetSPatientByIdAsync(createdPatient);
                return Ok(infosPatientCreated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
        [HttpGet("listById/{id}")]
        public async Task<IActionResult> GetSPatientByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest("ID inválido.");
                }

                var patient = await _patientUseCase.GetSPatientByIdAsync(id);
                if (patient == null)
                {
                    return NotFound($"Paciente com ID {id} não encontrada.");
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno do servidor: {ex.Message}");
            }
        }
    }
}