# OPEN WEATHER NET API

Api que provee datos del tiempo de Open Weather Map, con persistencia en una base de datos SQL.

## Requisitos Previos

- [.NET Core 6](https://dotnet.microsoft.com/download/dotnet/6.0)

## Instalación

1. Clona este repositorio:

    ```bash
    git clone https://github.com/FernandoMG81/OpenWeatherAPI.git
    ```

2. Navega al directorio del proyecto:

    ```bash
    cd OpenWeatherAPI
    ```

3. Restaura las dependencias del proyecto:

    ```bash
    dotnet restore
    ```

4. Compila el proyecto:

    ```bash
    dotnet build
    ```

5. Ejecuta la aplicación:

    ```bash
    dotnet run
    ```

La API estará disponible en `https://localhost:7164` por defecto.


## Uso

### Obtener el último registro del clima para una ciudad

```http
GET https://localhost:7164/api/Weather/latest/{cityID}
```

### Obtener todos los registros del clima para una ciudad

```http
GET https://localhost:7164/api/Weather/allrecords/{cityID}
```

### Agregar un nuevo registro del clima
```http
POST https://localhost:7164/api/Weather/add
```
```
Content-Type: application/json
{
  "cityID": 0,
  "timestamp": 0
}
```

### Eliminar un registro del clima
```http
DELETE https://localhost:7164/api/Weather/delete
```
```
Content-Type: application/json
{
  "cityID": 0,
  "timestamp": 0
}
```
