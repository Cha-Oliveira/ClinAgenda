using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // Método assíncrono para excluir um status pelo ID.
        public async Task<int> DeleteStatusAsync(int id)
        {
            // Query SQL para deletar um status pelo ID.
            string query = @"
            DELETE FROM STATUS
            WHERE ID = @Id";

            var parameters = new { Id = id }; // Parâmetro da query.

            // Executa a query e retorna o número de linhas afetadas.
            var rowsAffected = await _connection.ExecuteAsync(query, parameters);

            return rowsAffected; // Retorna quantas linhas foram afetadas (1 se deletou, 0 se não encontrou o ID).
        }

        // Método assíncrono para inserir um novo status no banco.
        public async Task<int> InsertStatusAsync(SpecialtyInsertDTO statusInsertDTO)
        {
            // Query SQL para inserir um novo status e obter o ID gerado.
            string query = @"
            INSERT INTO STATUS (NAME) 
            VALUES (@Name);
            SELECT LAST_INSERT_ID();"; // Obtém o ID do último registro inserido.

            // Executa a query e retorna o ID do novo status.
            return await _connection.ExecuteScalarAsync<int>(query, statusInsertDTO);
        }

        // Método assíncrono para obter todos os status com paginação.
        public async Task<(int total, IEnumerable<SpecialtyDTO> specialtys)> GetAllAsync(int? itemsPerPage, int? page)
        {
            // Construção dinâmica da query base.
            var queryBase = new StringBuilder(@"
                FROM STATUS S WHERE 1 = 1"); // "1 = 1" é usado para facilitar adição de filtros dinâmicos.

            var parameters = new DynamicParameters(); // Objeto para armazenar os parâmetros da query.

            // Query para contar o número total de registros sem a paginação.
            var countQuery = $"SELECT COUNT(DISTINCT S.ID) {queryBase}";
            int total = await _connection.ExecuteScalarAsync<int>(countQuery, parameters);

            // Query para buscar os dados paginados.
            var dataQuery = $@"
            SELECT ID, 
            NAME
            {queryBase}
            LIMIT @Limit OFFSET @Offset";

            // Adiciona os parâmetros de paginação.
            parameters.Add("Limit", itemsPerPage);
            parameters.Add("Offset", (page - 1) * itemsPerPage);

            // Executa a consulta e retorna os resultados.
            var status = await _connection.QueryAsync<SpecialtyDTO>(dataQuery, parameters);

            return (total, status); // Retorna o total de registros e a lista de status paginada.
        }
    }
}