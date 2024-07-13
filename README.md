# README

## Format

* Config is in `.editorconfig`
* vim

    ```text
    # configure 'editorconfig/editorconfig-vim' and 'sbdchd/neoformat' plug-ins
    autocmd BufWritePre *.cs silent! :Neoformat
    ```

* vscode
  * configure `EditorConfig for VS Code` plug-in



## Configuration

* csharp/shop/Consts.cs

    ```cs
    // change this line to your local host name of the ASP.NET server
    private const string ImageBaseUrl = "http://scottmm.local:15001/";

    // add your next.js server's host:port as CORS_ORIGINS
    public static string[] CORS_ORIGINS = new[] { "http://10.1.196.5:3000", "http://localhost:3000", "http://scottmm.local:3000" };
    ```

* react/context/shop-context.tsx

    ```tsx
    // change this line to your local host name of the ASP.NET server
    const baseCsUrl = "http://scottmm.local:15001";
    ```

## How to run

* submodule update

    ```sh
    git submodule update --init --recursive
    ```

* react

    ```sh
    npm run dev
    ```

* csharp/shop (C#)

    ```sh
    dotnet run
    ```

* Grafana, Loki, Tempo, Cortex

    ```sh
    cd shopping-docker
    podman-compose up -d

    # to stop
    podman-compose stop

    # to start again
    podman-compose start
    ```

## Grafana

* Grafana datasource connections
  * cortex: <http://cortex:9009/api/prom>
  * loki: <http://loki:3100>
  * tempo: <http://tempo:3200>

## Ingestion

* csharp/shoppping
  * It sends to the following endpoints
  * Tracing and Metrics are sent over OTLP gRPC
  * Logs are sent over OTLP HTTP using Serilog

    ```cs
    // csharp/shop/Consts.cs
    public const string OTLP_GRPC_ENDPOINT = "http://localhost:4317/";
    public const string OTLP_HTTP_ENDPOINT = "http://localhost:4318/";

    ```

* react
  * TBD: Tracing and Logs are sent over OTLP HTTP
  * Metrics are not sent over OTLP Metrics, but included in OTLP Logging
