using LibreHardwareMonitor.Hardware;
using System.Text.Json;
using System.Net.Http;
using System.Text;

// --------------------------------------------------
// PARÁMETROS:
// sin parámetros → una vez
// solo número → loop cada X ms
// --webhook URL → enviar POST
// --------------------------------------------------

bool loop = false;
int updateMs = 1000;
string? webhook = null;

for (int i = 0; i < args.Length; i++)
{
    if (int.TryParse(args[i], out int parsed))
    {
        loop = true;
        updateMs = parsed;
    }
    else if (args[i] == "--webhook" && i + 1 < args.Length)
    {
        webhook = args[i + 1];
        i++;
    }
}

// --------------------------------------------------
// CONFIGURAR MONITOR
// --------------------------------------------------

Computer computer = new Computer
{
    IsCpuEnabled = true,
    IsGpuEnabled = true,
    IsMotherboardEnabled = true,
    IsMemoryEnabled = true,
    IsStorageEnabled = true
};

computer.Open();

HttpClient http = new HttpClient();

// --------------------------------------------------
// FUNCIÓN PARA LEER Y CONSTRUIR JSON
// --------------------------------------------------

string ReadJson()
{
    var list = new List<object>();

    foreach (var hw in computer.Hardware)
    {
        hw.Update();

        foreach (var s in hw.Sensors)
        {
            list.Add(new
            {
                hardware = hw.Name,
                type = s.SensorType.ToString(),
                name = s.Name,
                value = FixValue(s.Value)
            });
        }
    }

    return JsonSerializer.Serialize(list, new JsonSerializerOptions
    {
        WriteIndented = false
    });
}

// --------------------------------------------------
// FUNCIÓN PARA ENVIAR WEBHOOK
// --------------------------------------------------

async Task SendWebhook(string url, string json)
{
    try
    {
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        await http.PostAsync(url, content);
    }
    catch
    {
        // No detener el programa si falla
    }
}


// --------------------------------------------------
// UNA SOLA LECTURA (sin parámetros numéricos)
// --------------------------------------------------

if (!loop)
{
    string json = ReadJson();
    Console.WriteLine(json);

    if (webhook != null)
        await SendWebhook(webhook, json);

    computer.Close();
    return;
}


// --------------------------------------------------
// LOOP INFINITO (con parámetro numérico)
// --------------------------------------------------

while (true)
{
    string json = ReadJson();
    Console.WriteLine(json);

    if (webhook != null)
        await SendWebhook(webhook, json);

    Thread.Sleep(updateMs);
}


// --------------------------------------------------
// LIMPIAR NaN / Infinity
// --------------------------------------------------

static float? FixValue(float? val)
{
    if (val == null) return null;

    float v = val.Value;

    if (float.IsNaN(v) || float.IsInfinity(v))
        return null;

    return v;
}
