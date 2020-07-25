# Generate PDF from Azure Functions

## PdfHelloWorld

A simple test PDF generation.

- Header
- Footer
- Watermark

```bash
curl https://<function-app-name>.azurewebsites.net/api/PdfHelloWorld
```

## PdfFromUrl

Generate PDF from specified URL

```bash
curl -X POST -H "Content-Type: application/json" -v -d '{"url":"https://docs.microsoft.com/zh-tw/"}' https://<function-app-name>.azurewebsites.net/api/PdfFromUrl 

curl -v https://<function-app-name>.azurewebsites.net/api/PdfFromUrl?url=https://docs.microsoft.com/zh-tw/
```

## PdfFromHtml

Generate PDF from request body  (HTML)

```bash
curl -X POST -H "Content-Type: application/json" -v -d '<h1>Hello World... now what?</h1>' https://<function-app-name>.azurewebsites.net/api/PdfFromHtml
```
