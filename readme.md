# kllmp.org.msql

## Descripción general

La clase Sql proporciona una implementación para el acceso a datos en SQL Server, permitiendo ejecutar comandos SQL, obtener resultados en diferentes formatos y mapearlos a objetos fuertemente tipados. Incluye utilidades para configuración y mapeo eficiente de resultados.



## Constructores

- **Sql(string connectionString)** - Inicializa una nueva instancia con la cadena de conexión especificada.
- **Sql(string connectionString, int commandTimeout)** - Inicializa una nueva instancia con la cadena de conexión y el tiempo de espera del comando.
- **Sql(string connectionString, CommandType commandType)** - Inicializa una nueva instancia con la cadena de conexión y el tipo de comando.
- **Sql(string connectionString, int commandTimeout, CommandType commandType)** - Inicializa una nueva instancia con la cadena de conexión, el tiempo de espera y el tipo de comando.



## Métodos de configuración

- **SetConnectionString(string connectionString)** - Establece la cadena de conexión para las operaciones SQL.
- **SetCommandType(CommandType commandType)** - Establece el tipo de comando SQL a utilizar.
- **SetCommandTimeout(int commandTimeout)** - Establece el tiempo de espera del comando en segundos.
- **AddCommandType(CommandType commandType)** - Permite establecer el tipo de comando y encadenar la configuración.
- **AddCommandTimeout(int commandTimeout)** - Permite establecer el tiempo de espera y encadenar la configuración.
- **AddSettings(string connectionString, int commandTimeout, CommandType commandType)** - Permite establecer todos los parámetros principales y encadenar la configuración.
- **ISql Build()** - Valida la configuración y devuelve la instancia lista para usar.



## Métodos principales

- **int Exec(string query, SqlParameter[]? parameters = null)** - Ejecuta un comando SQL y devuelve el número de filas afectadas.
- **DataTable ExecDataTable(string query, SqlParameter[]? parameters = null)** - Ejecuta un comando SQL y devuelve los resultados en un DataTable.
- **DataSet ExecDataSet(string query, SqlParameter[]? parameters = null)** - Ejecuta un comando SQL y devuelve los resultados en un DataSet.
- **List<T> ExecDataList<T>(string query, SqlParameter[]? parameters = null)** - Ejecuta un comando SQL y devuelve una lista de objetos de tipo T, mapeando las columnas a las propiedades de la clase.
- **IEnumerable<T> LazyExecute<T>(string query, params SqlParameter[] parameters)** - Ejecuta un comando SQL y devuelve una colección enumerable de objetos de tipo T. Los resultados se leen bajo demanda (ejecución diferida).



## Utilidades

- **static T DBRowToObject<T>(DataRow row)** - Convierte un DataRow en un objeto de tipo T, mapeando columnas a propiedades.
- **static int DBInt(object value)** - Convierte un valor a int, devolviendo 0 si es DBNull.
- **static long DBLong(object value)** - Convierte un valor a long, devolviendo 0 si es DBNull.
- **static string DBString(object value)** - Convierte un valor a string, devolviendo cadena vacía si es DBNull.
- **static decimal DBDecimal(object value)** - Convierte un valor a decimal, devolviendo 0 si es DBNull.
- **static bool DBBool(object value)** - Convierte un valor a bool, devolviendo false si es DBNull.




### Metodos de conversion
Ejemplo para la conversion de datos.

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