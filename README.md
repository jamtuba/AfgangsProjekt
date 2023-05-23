[![Build and deploy Azure Timer Function - Get Stock Prices](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/Timer_Function_ci_cd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/Timer_Function_ci_cd.yml)
<br/>
[![Azure Static Web Apps CI/CD](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/blazorwasm-cicd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/blazorwasm-cicd.yml)
<br/>
[![WEBAPI CI/CD](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/webapi-cicd.yml/badge.svg)](https://github.com/jamtuba/AfgangsProjekt/actions/workflows/webapi-cicd.yml)

# AfgangsProjekt
### Diagram over de forskellige services i projektet og flowet imellem dem.

![system diagram](Diagrams/AP_System_Diagram_170523.jpg "Afgangs projekt system diagram")

Kode til de forskellige services i forbindelse med mit afgangsprojekt på SmartLearning. <br />
For at koden og tests kan køre skal koden bygges først.

Min løsning på afgangsprojektet består af en række projekter med hver sin funktionalitet.

- AP.API er et minimalt webapi der henter beskeder fra CloudAMQP og sender dem videre med SignalR. <br />
  Her er der 2 interfaces IRabbitMQConsumer og IRabbitMQService. <br />
  Der er også min SignalR hub, "thehub".

- AP.APITests indeholder nogle tests for API´et.

- AP.BlazorWASM er projektet med min frontend SPA. <br />
  Det indeholder kun en side, Index.razor.

- AP.BlazorWASMTests indeholder tests til min BlazorWASM.

- AP.ClassLibrary er mit classlibrary med fællesklasser og metoder til brug i hele løsningen. <br />
  Her er også statiske endpoints.

- AP.FuntionTests er tests til min Azure Function applikation og dens afhængigheder.

- AP.GetStockPrices er min Azure Function med en Timer Function og de 2 interfaces IWebscraperService og IRabbitMQPublisherService. <br />
  Herfra bliver der sendt aktiekurser videre til CloudAMQP som beskeder.
<br />
<br />

### Der er også mulighed for at se de øvrige diagrammer:
<br />

Enterpise Integration Patterns diagram

![EIP diagram](Diagrams/AP_EIP_Flow_Diagram_1.jpg "Afgangs projekt EIP flow diagram")
<br />
<br />
Azure Timer Function komponent diagram

![Azure Timer Function component diagram](Diagrams/Azure_Timer_Function_Diagram.jpg "Afgangs projekt Azure Function komponent diagram")
<br />
<br />
Minimal Web API komponent diagram

![Minimal Web API component diagram](Diagrams/Minimal_API_Diagram.jpg "Afgangs projekt Minimal Web API komponent diagram")