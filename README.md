SqlBuilder
=======

Original computation expression found from http://cs.hubfs.net/topic/None/58747. I added transaction support

Here is a sample

```f#
open System
open DB
 
let successful =  DB.transaction {
                        do! @"
                                INSERT INTO Users(
                                        UserGuid,
                                        Active,
                                        Test,
                                        UserName,
                                        PasswordHash,
                                        FirstName,
                                        MiddleName,
                                        LastName
                                )
                                VALUES (
                                    '45cd56789012345678901234567890ef',
                                    0,
                                    0,
                                    'F#erpnator3',
                                    '8949f7c11dc819e363e5c63242ef31d2a1a845d15daaf0cc8c2d49c56950a1e0',
                                    'test',
                                    'test',
                                    'test'
                                )"
                  }

let users = DB.queryDb {
                    let! u = "Select * from users where userguid = '45cd56789012345678901234567890ef'"
                    return u
                }

Console.ReadKey()
```