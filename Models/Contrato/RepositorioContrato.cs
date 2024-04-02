using MySql.Data.MySqlClient;


namespace Tp_Inmobiliaria_Ledesma_Lillo.Models;

public class RepositorioContrato
{

    protected readonly string connectionString = "Server=localhost;Database=ledesmalillo;User=root;Password=;";
    public RepositorioContrato()
    {

    }
    public int AltaContrato(Contrato c)
    {
        int id = -1;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @$"INSERT INTO Contratos ({nameof(Contrato.InmuebleId)},
                     {nameof(Contrato.InquilinoId)},
                     {nameof(Contrato.FecInicio)},
                     {nameof(Contrato.FecFin)},
                     {nameof(Contrato.Monto)},
                     {nameof(Contrato.Estado)}
                       
					VALUES (@{nameof(Contrato.InmuebleId)},
                    @{nameof(Contrato.InquilinoId)},
                    @{nameof(Contrato.FecInicio)},
                    @{nameof(Contrato.FecFin)},
                    @{nameof(Contrato.Monto)},
                    @{nameof(Contrato.Estado)}) 
					SELECT LAST_INSERT_ID();";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.InmuebleId)}", c.InmuebleId);
                command.Parameters.AddWithValue($"@{nameof(Contrato.InquilinoId)}", c.InquilinoId);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FecInicio)}", c.FecInicio);
                command.Parameters.AddWithValue($"@{nameof(Contrato.FecFin)}", c.FecFin);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Monto)}", c.Monto);
                command.Parameters.AddWithValue($"@{nameof(Contrato.Estado)}", c.Estado);
                connection.Open();
                id = Convert.ToInt32(command.ExecuteScalar());
                c.IdContrato = id;
                connection.Close();
            }
        }
        return id;
    }

    public int ModificaContrato(Contrato c)
    {
        using (var connection = new MySqlConnection(connectionString))
        {

            var sql = @$"UPDATE contratos 
            SET 
            {nameof(Contrato.InmuebleId)} = @{nameof(Contrato.InmuebleId)},
            {nameof(Contrato.InquilinoId)} = @{nameof(Contrato.InquilinoId)},
            {nameof(Contrato.FecInicio)} = @{nameof(Contrato.FecInicio)},
            {nameof(Contrato.FecFin)} = @{nameof(Contrato.FecFin)},
            {nameof(Contrato.Monto)} = @{nameof(Contrato.Monto)},
            {nameof(Contrato.Estado)} = @{nameof(Contrato.Estado)}
            WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)}";

            using (var command = new MySqlCommand(sql, connection))
            {

                command.Parameters.AddWithValue($@"{nameof(Contrato.IdContrato)}", c.IdContrato);
                command.Parameters.AddWithValue($@"{nameof(Contrato.InquilinoId)}", c.InquilinoId);
                command.Parameters.AddWithValue($@"{nameof(Contrato.FecInicio)}", c.FecInicio);
                command.Parameters.AddWithValue($@"{nameof(Contrato.FecFin)}", c.FecFin);
                command.Parameters.AddWithValue($@"{nameof(Contrato.Monto)}", c.Monto);
                command.Parameters.AddWithValue($@"{nameof(Contrato.Estado)}", c.Estado);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }

        }

        return 0;
    }

    public int EliminaContrato(int id)
    {

        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @$"DELETE FROM contratos
                    WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        return 0;

    }

    public Contrato? ObtenerContrato(int id)
    {
        Contrato? contrato = null;
        using (var connection = new MySqlConnection(connectionString))
        {
            var sql = @$"SELECT 
            {nameof(Contrato.IdContrato)}, {nameof(Contrato.InmuebleId)},
            {nameof(Contrato.InquilinoId)}, {nameof(Contrato.FecInicio)},
            {nameof(Contrato.FecFin)}, {nameof(Contrato.Monto)}, {nameof(Contrato.Estado)}
            
            FROM contratos c INNER JOIN inquilinos inq ON c.InquilinoId = inq.IdInquilino
            INNER JOIN inmuebles i ON c.InmuebleId = i.IdInmueble
            WHERE {nameof(Contrato.IdContrato)} = @{nameof(Contrato.IdContrato)}";
            using (var command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue($"@{nameof(Contrato.IdContrato)}", id);
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        contrato = new Contrato
                        {
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
                            InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
                            FecInicio = reader.GetDateTime(nameof(Contrato.FecInicio)),
                            FecFin = reader.GetDateTime(nameof(Contrato.FecFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            Inquilino = new Inquilino
                            {
                                IdInquilino = reader.GetInt32(nameof(Contrato.InquilinoId)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido))
                            },
                            Inmueble = new Inmueble
                            {
                                IdInmueble = reader.GetInt32(nameof(Contrato.InmuebleId)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion)),
                                Ambientes = reader.GetInt32(nameof(Inmueble.Ambientes)),
                                Uso = reader.GetString(nameof(Inmueble.Uso))
                            }

                        };

                    }

                }
                connection.Close();

            }

        }

        return contrato;
    }

    public IList<Contrato> ObtenerContratos(){
        IList<Contrato> contratos = new List<Contrato>();

        using (var connection = new MySqlConnection(connectionString)){

            var sql = @$" SELECT {nameof(Inmueble.Direccion)}, {nameof(Inquilino.Nombre)},
            {nameof(Inquilino.Apellido)}, {nameof(Contrato.FecInicio)}, {nameof(Contrato.FecFin)},
            {nameof(Contrato.Monto)}, {nameof(Contrato.Estado)}, {nameof(Contrato.IdContrato)}
            FROM contratos c INNER JOIN inquilinos inq ON c.InquilinoId = inq.IdInquilino
            INNER JOIN inmuebles i ON c.InmuebleId = i.IdInmueble";

            using(var command = new MySqlCommand(sql, connection)){
                connection.Open();
                using(var reader = command.ExecuteReader())
                    while(reader.Read()){
                        contratos.Add(new Contrato{
                            IdContrato = reader.GetInt32(nameof(Contrato.IdContrato)),
                            InquilinoId = reader.GetInt32(nameof(Contrato.InquilinoId)),
                            InmuebleId = reader.GetInt32(nameof(Contrato.InmuebleId)),
                            FecInicio = reader.GetDateTime(nameof(Contrato.FecInicio)),
                            FecFin = reader.GetDateTime(nameof(Contrato.FecFin)),
                            Monto = reader.GetDecimal(nameof(Contrato.Monto)),
                            Estado = reader.GetBoolean(nameof(Contrato.Estado)),
                            Inquilino = new Inquilino{
                                IdInquilino = reader.GetInt32(nameof(Contrato.InquilinoId)),
                                Nombre = reader.GetString(nameof(Inquilino.Nombre)),
                                Apellido = reader.GetString(nameof(Inquilino.Apellido))
                            },
                            Inmueble = new Inmueble{
                                IdInmueble = reader.GetInt32(nameof(Contrato.InmuebleId)),
                                Direccion = reader.GetString(nameof(Inmueble.Direccion))
                            }
                        });
                    }
                connection.Close();
            }
        }
        return contratos;
    }
}