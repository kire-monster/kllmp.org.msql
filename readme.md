# kllmp.org.msql

## Descripcion

Modulo que permite conectar a bases de datos SQL Server. Se puede configurar para ejecutar los querys en modo texto o procedimientos almacenados.


## Constructores

Se puede usar cualquiera de los siguientes constructores:

- Sql(string ConnectionString)
- Sql(string ConnectionString, int CommandTimeout)
- Sql(string ConnectionString, CommandType CommandType)
- Sql(string ConnectionString, int CommandTimeout, CommandType CommandType)

## Metodos
Los metodos principales que podemos utilizar son los siquientes:
- Exec
- ExecDataTable
- ExecDataSet
- ExecDataList
- DBRowToObject
- DBInt
- DBString
- DBDecimal
- DBLong
- DBBool


A continuacion se detalla más cada metodo

### Exec
Permite ejecutar querys o procedimientos, generalmente de tipo insercion, actualizacion y eliminacion, como valor de retorno devueve un entero.

```
int Exec(string query, SqlParameter[]? parameters = null)
```

### ExecDataTable
Ejecuta query o procedimientos, mayormente de consulta los resultados los presenta en DataTable.

```
DataTable ExecDataTable(string query, SqlParameter[]? parameters = null)
```

### ExecDataSet
Ejecuta query o procedimientos, mayormente de consulta los resultados los presenta en DataSet.

```
DataSet ExecDataSet(string query, SqlParameter[]? parameters = null)
```

### ExecDataList
Ejecuta query o procedimientos, mayormente de consulta los resultados los presenta en una lista de un objecto. este objeto (clase) debe tener los atributos similares a los devuetos de la consulta, para que pueda asignarlos de forma correcta.

```
List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null)
```

### Metodos de conversion
Se añaden metodos de conversion, esto con la finalidad de hacer mas dinamica la generacion de objectos.

```
ClaseConCampos objeto = DBRowToObject<ClaseConCampos>(dataRow);
int entero = DBInt(row["nameColumn"]);
string cadena = DBString(row["nameColumn"]);
decimal valorDecimal = DBDecimal(row["nameColumn"]);
long valorLong = DBLong(row["nameColumn"]);
bool flag = DBBool(row["nameColumn"]);
```

para el metodo DBRowToObject los atributos de la clase deben coincidir con el nombre de columnas, ignora si son mayusculas o minusculas ejemplo:

```
// un archivo prueba.cs
class Prueba
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public string? Fecha_Nacimiento { get; set; }
}

// la logica
using System.Data;
using kllmp.org.msql;

const string ConnectionString = "tu cadena de conexion";

var con = new Sql(ConnectionString);
string query = "select id, nombre, fecha_nacimiento from tbl_Prueba";
var table = con.ExecDataTable(query);
var resultado = table
    .AsEnumerable()
    .Where(row => Sql.DBInt(row["id"]) == 1) // convertimos el row["id"] en tipo int
    .Select(row => Sql.DBRowToObject<Prueba>(row)) // convertimos el DataRow en una clase
    .First();
```