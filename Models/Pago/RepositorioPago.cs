using MySql.Data.MySqlClient;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Models;

public class RepositorioPago
{
    protected readonly string connectionString = "Server=localhost;Database=ledesmalillo;User=root;Password=;";

    public RepositorioPago()
    {

    }

    public IList<Pago> ObtenerTodosPagos()
    {
			var pagos = new List<Pago>();

			using(var connection = new MySqlConnection(connectionString))
            {
                var sql = @$"Select {nameof(Pago.NumPago)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)},
                                {nameof(Pago.ContratoId)},  
                            from pagos";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using(var reader = command.ExecuteReader())

                    while(reader.Read())
                    {
                        pagos.Add(new Pago{
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            NumPago = reader.GetInt32(nameof(Pago.NumPago)),
                            FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                            Importe = reader.GetDecimal(nameof(Pago.Importe)),
                            ContratoId = reader.GetInt32(nameof(Pago.ContratoId)),
                        });
                    }
                    connection.Close();
                }
                return pagos;
            }
	}

    public Pago ObtenerPago(int id)
    {
			Pago? pago = null;

			using(var connection = new MySqlConnection(connectionString))
            {
                var sql = @$"Select {nameof(Pago.NumPago)}, {nameof(Pago.FechaPago)}, {nameof(Pago.Importe)},
                                {nameof(Pago.ContratoId)},  
                            from pagos
                            WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)}";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
                    connection.Open();
                    using(var reader = command.ExecuteReader())

                    if(reader.Read())
                    {
                        pago = new Pago{
                            IdPago = reader.GetInt32(nameof(Pago.IdPago)),
                            NumPago = reader.GetInt32(nameof(Pago.NumPago)),
                            FechaPago = reader.GetDateTime(nameof(Pago.FechaPago)),
                            Importe = reader.GetDecimal(nameof(Pago.Importe)),
                            ContratoId = reader.GetInt32(nameof(Pago.ContratoId)),
                        };
                    }
                    connection.Close();
                }
                return pago;
            }
	}

    public int AltaPago(Pago pago)
		{
			int id = 0;
		using(var connection = new MySqlConnection(connectionString))
		{
			var sql = @$"INSERT INTO pagos ({nameof(Pago.NumPago)}, {nameof(Pago.Importe)}, {nameof(Pago.FechaPago)}
                        {nameof(Pago.ContratoId)})
				VALUES (@{nameof(Pago.NumPago)},
                         {nameof(Pago.Importe)},
                         {nameof(Pago.FechaPago)},
                         {nameof(Pago.ContratoId)});
				SELECT LAST_INSERT_ID();";
			using(var command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(Pago.NumPago)}", pago.NumPago);
                command.Parameters.AddWithValue($"@{nameof(Pago.Importe)}", pago.Importe);
                command.Parameters.AddWithValue($"@{nameof(Pago.FechaPago)}", pago.FechaPago);
                command.Parameters.AddWithValue($"@{nameof(Pago.ContratoId)}", pago.ContratoId);

				connection.Open();
				id = Convert.ToInt32(command.ExecuteScalar());
				pago.IdPago = id;
				connection.Close();
			}
		}
		return id;
	}

     public int EliminaPago(int id)
	{
		using(var connection = new MySqlConnection(connectionString))
		{
			var sql = @$"DELETE FROM pagos
				WHERE {nameof(Pago.IdPago)} = @{nameof(Pago.IdPago)}";
			using(var command = new MySqlCommand(sql, connection))
			{
				command.Parameters.AddWithValue($"@{nameof(Pago.IdPago)}", id);
				connection.Open();
				command.ExecuteNonQuery();
				connection.Close();
			}
		}
		return 0;
	}
}