// Helper for downloading files returned by APIs
// Usage from Blazor: JS.InvokeVoidAsync("downloadFile", fileName, contentType, base64Data)
window.downloadFile = (fileName, contentType, base64Data) => {
  const linkSource = `data:${contentType};base64,${base64Data}`;
  const downloadLink = document.createElement("a");
  downloadLink.href = linkSource;
  downloadLink.download = fileName;
  document.body.appendChild(downloadLink);
  downloadLink.click();
  document.body.removeChild(downloadLink);
};

