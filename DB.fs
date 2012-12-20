module DB
open System
open System.IO
open System.Data.SQLite
open System.Data 
open System.Data.Common

let startText = ""

let commitText = ""

type SQLBuilder(conn: DbConnection, createTransaction) = 
    
    let getFields (reader: DbDataReader) = 
        [| for i in 0 .. reader.FieldCount - 1 -> 
            let readerValue = reader.[ i ]
            let readerName = reader.GetName(i)
            (readerName, readerValue) |]

    do 
        conn.Open()
 
    member this.Zero () = ()

    member b.Connection = conn
 
    member b.Bind (expr, rest) =
        // get a single row
        use cmd = conn.CreateCommand(CommandText = expr)
 
        use reader = cmd.ExecuteReader(CommandBehavior.SingleRow)
 
        rest (getFields reader)
 
    member b.For (expr, rest) =
        // get all rows
        use cmd = conn.CreateCommand(CommandText = expr)
        use reader = cmd.ExecuteReader()
        while reader.Read() do
            rest (getFields reader)
 
    member b.Return expr =
        expr
 
    member b.Combine (expr1, expr2) =
       expr1
       expr2
 
    member b.Delay (expr) =
        expr ()



type Transaction(conn:IDbConnection) = 
    member this.executeQuery query = 
        use cmd = conn.CreateCommand(CommandText = query)
        cmd.ExecuteNonQuery() |> ignore
        true
                                
    member this.Bind(value, expr) = 
        if this.executeQuery value then
            expr()
            true
        else 
            false
        
    member this.Return expr = expr

    member this.wrapInTransaction expr = 
            use trans = conn.BeginTransaction()
            this.executeQuery startText |> ignore
            try
                let success = expr()

                this.executeQuery commitText |> ignore

                trans.Commit()

                true
            with 
                | :? System.Exception as ex -> Console.WriteLine ("an error occurred {0}", ex)
                                               false

                           
    member this.Yield item = item

    member this.Run expr = this.wrapInTransaction expr

    member this.Delay expr = fun() -> expr()
                                

let connection = new SQLiteConnection((sprintf "Data Source = %s" @"C:\db\db.dat"))

let transaction = new Transaction(connection)

let queryDb = SQLBuilder(connection, false)