using System.Net;
using System.Text;
using System.Text.Json;

HttpListener httpListener = new HttpListener();
httpListener.Prefixes.Add("http://localhost:8080/");
httpListener.Start();

Console.WriteLine("Listeninig...");

while (true)
{
    var context = await httpListener.GetContextAsync();
    Console.WriteLine(context.Request.QueryString["id"]);
    if (context.Request.HttpMethod == "POST")
    {
        var body = new StreamReader(context.Request.InputStream).ReadToEnd();
        var data = await JsonSerializer.DeserializeAsync<MailForm>(context.Request.InputStream);
        Console.WriteLine(body);
    }

    var response = context.Response;
    string testResponse = @"
<!DOCTYPE html>
<html>
    <head>
        <meta charset='utf8'>
        <title>HELLO Http</title>
    </head>
    <body>
        <h1>Uraaaaaaaaa</h1>
        <form>
            <label for='From'>From:</label>
            <input type='text' id='from' name='From'><br><br>
            <label for='to'>To:</label>
            <input type='text' id='to' name='to'><br><br>
            <label for='to'>Message:</label>
            <textarea id='message' name='Message' rows='4' cols='50'></textarea><br><br>
            <button onclick='sendData()'>Send</button>
        </from>
        <script>
            function sendData()
            {
                const to = document.getElementById('to').value;
                const from = document.getElementById('from').value;
                const message = document.getElementById('message').value;

                fetch('http://localhost:8080/', {method: 'POST', headers: {'Content-Type': 'application/json'}, body: JSON.stringify({to, from, message})})
                .then(response => response.json())
                .then(result => console.log(result))
                .catch(error => console.log('error', error));
            }
        </script>
    </body>
</html>";
    byte[] buffer = Encoding.UTF8.GetBytes(testResponse);
    response.ContentLength64 = buffer.Length;
    using Stream stram = response.OutputStream;
    await stram.WriteAsync(buffer);
    await stram.FlushAsync();
    Console.WriteLine(context.Request.Url);
}

public class MailForm
{
    public string To { get; set; }
    public string From { get; set; }
    public string Message { get; set; }

    public MailForm(string to, string from, string message)
    {
        To = to;
        From = from;
        Message = message;
    }
}

