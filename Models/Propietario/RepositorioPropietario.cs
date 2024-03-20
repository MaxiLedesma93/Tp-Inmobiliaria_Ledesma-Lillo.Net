using MySql.Data.MySqlClient;
using System.Configuration;
using System.Data;
namespace Tp_Inmobiliaria_Ledesma_Lillo.Models;

public class RepositorioPropietario
{
    protected readonly string connectionString = "Server=localhost;Database=ledesmalillo;User=root;Password=;";
		public RepositorioPropietario()
		{
			
		}

		public IList<Propietario> ObtenerPropietarios()
		{
			var propietarios = new List<Propietario>();

			using(var connection = new MySqlConnection(connectionString))
        {
            var sql = @$"Select {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, 
                         {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)} 
						  from propietarios";
            using (var command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using(var reader = command.ExecuteReader())

                while(reader.Read())
                {
                    propietarios.Add(new Propietario{
                        IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),
						//Clave = reader.GetString(nameof(Propietario.Clave)),
                    });
                }
            }
            return propietarios;
        }
		}

		public Propietario? ObtenerPropietario(int id)
		{
			Propietario? propietario = null;

			using(var connection = new MySqlConnection(connectionString))
        {
            var sql = @$"Select {nameof(Propietario.IdPropietario)}, {nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, 
                         {nameof(Propietario.Dni)}, {nameof(Propietario.Telefono)}, {nameof(Propietario.Email)} 
						  from propietarios 
						 where {nameof(Propietario.IdPropietario)} = @id";
            using (var command = new MySqlCommand(sql, connection))
            {
				command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}",id);
                connection.Open();
                using(var reader = command.ExecuteReader())

                if(reader.Read())
                {
                    propietario = new Propietario{
                        IdPropietario = reader.GetInt32(nameof(Propietario.IdPropietario)),
						Nombre = reader.GetString(nameof(Propietario.Nombre)),
						Apellido = reader.GetString(nameof(Propietario.Apellido)),
						Dni = reader.GetString(nameof(Propietario.Dni)),
						Telefono = reader.GetString(nameof(Propietario.Telefono)),
						Email = reader.GetString(nameof(Propietario.Email)),
						//Clave = reader.GetString(nameof(Propietario.Clave)),
                    };
                }
            }
            return propietario;
        }
		}

		public int AltaPropietario(Propietario propietario)
		{
			int id = 0;
		using(var connection = new MySqlConnection(connectionString))
		{
			var sql = @$"INSERT INTO propietarios ({nameof(Propietario.Nombre)}, {nameof(Propietario.Apellido)}, {nameof(Propietario.Dni)},
			{nameof(Propietario.Telefono)}, {nameof(Propietario.Email)})
				VALUES (@{nameof(Propietario.Nombre)}, @{nameof(Propietario.Apellido)}, @{nameof(Propietario.Dni)},
				@{nameof(Propietario.Telefono)}, @{nameof(Propietario.Email)});
				SELECT LAST_INSERT_ID();";
			using(var command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Dni)}", propietario.Dni);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Email)}", propietario.Email);
				//command.Parameters.AddWithValue($"@{nameof(Propietario.Clave)}", propietario.Clave);

				connection.Open();
				id = Convert.ToInt32(command.ExecuteScalar());
				propietario.IdPropietario = id;
				connection.Close();
			}
		}
		return id;
		}

		public int ModificaPropietario(Propietario propietario)
	{
		using(var connection = new MySqlConnection(connectionString))
		{
			var sql = @$"UPDATE propietarios
				SET {nameof(Propietario.Nombre)} = @{nameof(Propietario.Nombre)},
				{nameof(Propietario.Apellido)} = @{nameof(Propietario.Apellido)},
				{nameof(Propietario.Dni)} = @{nameof(Propietario.Dni)},
				{nameof(Propietario.Telefono)} = @{nameof(Propietario.Telefono)},
				{nameof(Propietario.Email)} = @{nameof(Propietario.Email)},
				
				WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
			using(var command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(Propietario.Nombre)}", propietario.Nombre);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Apellido)}", propietario.Apellido);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Dni)}", propietario.Dni);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Telefono)}", propietario.Telefono);
				command.Parameters.AddWithValue($"@{nameof(Propietario.Email)}", propietario.Email);
				//command.Parameters.AddWithValue($"@{nameof(Propietario.Clave)}", propietario.Clave);
				connection.Open();
				command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return 0;
	}


	public int EliminaPersona(int id)
	{
		using(var connection = new MySqlConnection(connectionString))
		{
			var sql = @$"DELETE FROM propietarios
				WHERE {nameof(Propietario.IdPropietario)} = @{nameof(Propietario.IdPropietario)}";
			using(var command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(Propietario.IdPropietario)}", id);
				connection.Open();
				command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return 0;
	}
}