using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClinAgenda.src.Application.DTOs.Specialty;
using Dapper;
using MySql.Data.MySqlClient;

namespace ClinAgenda.src.Infrastructure.Repositories
{
    public class SpecialtyRepository
    {
        private readonly MySqlConnection _connection; // Conexão com o banco de dados.

        // Construtor que recebe a conexão com o banco de dados via injeção de dependência.
        public SpecialtyRepository(MySqlConnection connection)
        {
            _connection = connection;
        }

         // Método assíncrono para buscar um status pelo ID.
        public async Task<SpecialtyDTO> GetByIdAsync(int id)
        {
            // Query SQL para selecionar o status pelo ID.
            string query = @"
            SELECT ID, 
                   NAME 
            FROM STATUS
            WHERE ID = @Id";

            var parameters = new { Id = id }; // Parâmetro da query.

            // Executa a consulta no banco e retorna o primeiro resultado ou null caso não encontre.
            var status = await _connection.QueryFirstOrDefaultAsync<SpecialtyDTO>(query, parameters);

            return status; // Retorna o status encontrado.
        }

        
    }
}