[![Build and deploy Azure Timer Function - Get Stock Prices](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/Timer_Function_ci_cd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/Timer_Function_ci_cd.yml)
<br/>
[![Azure Static Web Apps CI/CD](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/blazorwasm-cicd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/blazorwasm-cicd.yml)
<br/>
[![WEBAPI CI/CD](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/webapi-cicd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/webapi-cicd.yml)

# AfgangsProjekt
### Diagram over de forskellige services i projektet og flowet imellem dem.

![Alt text](Diagrams/AP_System_Diagram_170523.jpg "Title")

Kode til forskellige services i forbindelse med mit afgangsprojekt på SmartLearning.

Min løsning på afgangsprojektet består af en række projekter med hver sin funktionalitet.

AP.API er et minimalt webapi der henter beskeder fra CloudAMQP og sender dem videre med SignalR.
Her er der 2 interfaces IRabbitMQConsumer og IRabbitMQService.
Der er også min SignalR hub, "thehub".

AP.APITests indeholder nogle tests for API´et.

AP.BlazorWASM er projektet med min frontend SPA.
Det indeholder kun en side, Index.razor.

AP.BlazorWASMTests indeholder tests til min BlazorWASM.

AP.ClassLibrary er mit classlibrary med fællesklasser og metoder til brug i hele løsningen. 
Her er også statiske endpoints.

AP.FuntionTests er tests til min Azure Function applikation og dens afhængigheder.

AP.GetStockPrices er min Azure Function med en Timer Function og de 2 interfaces IWebscraperService og IRabbitMQPublisherService.




<!---

    PREVIEW Ctrl+Shift+V

---!>