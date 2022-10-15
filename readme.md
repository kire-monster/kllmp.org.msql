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