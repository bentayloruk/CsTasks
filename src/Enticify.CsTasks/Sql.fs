module CsTasks.Sql

open System.Data.SqlClient
open System.Data.OleDb

let StripProvider connectionString =
    let builder = OleDbConnectionStringBuilder(connectionString);
    let removed = builder.Remove("Provider")
    let removed = builder.Remove("provider")
    builder.ConnectionString

//SqlHelper lifted from this snippet http://fssnip.net/ac
type SqlHelper (connection) =
    let exec bind parametres query = 
        use conn = new SqlConnection (connection)
        conn.Open()
        use cmd = new SqlCommand (query, conn)
        parametres |> List.iteri (fun i p -> 
                        cmd.Parameters.AddWithValue(sprintf "@p%d"  i, box p) |> ignore)
        bind cmd

    member __.Execute = exec <| fun c -> c.ExecuteNonQuery() |> ignore
    member __.Scalar  = exec <| fun c -> c.ExecuteScalar()
    member __.Read f  = exec <| fun c -> [ let read = c.ExecuteReader()
                                           while read.Read() do 
                                               yield f read ]