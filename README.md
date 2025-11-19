# HardwareMonitorCLI

Herramienta de consola para leer sensores de hardware (CPU, GPU, RAM, placa madre, discos, etc.) usando LibreHardwareMonitor y generar salida JSON.

Opcionalmente puede ejecutar en loop y enviar los datos a un webhook vía POST.

---

## Características

- Obtiene datos de CPU, GPU, RAM, motherboard y almacenamiento.  
- Salida JSON válida.  
- Modo una sola ejecución (sin parámetros).  
- Modo loop indicando un número en milisegundos.  
- Webhook opcional usando `--webhook URL`.  
- Limpieza automática de valores inválidos (NaN, Infinity).  
- Todo en un único `.exe` auto-contenido (opcional).

---

## Instalación / Compilación

1. Agregar dependencia:

dotnet add package LibreHardwareMonitor



2. Compilar:

dotnet build -c Release


3. (Opcional) Generar EXE único:

dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true


El EXE estará en:  
`bin/Release/net8.0/win-x64/publish/`

---

## Uso

Ejecutar una sola vez:

HardwareMonitorCLI.exe


Ejecutar cada X ms:

HardwareMonitorCLI.exe 2000


Enviar a webhook:

HardwareMonitorCLI.exe --webhook https://miweb.com/hook


Loop + webhook:

HardwareMonitorCLI.exe 1000 --webhook http://localhost:3000/data

---

## Parámetros

- Sin parámetros → ejecuta una sola vez  
- Número → ejecuta cada X milisegundos  
- `--webhook URL` → envía POST con los datos  
