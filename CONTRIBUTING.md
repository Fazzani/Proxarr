# How to contribute

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## Prerequisites

1. Before the project can be built, you must first install the .NET 9.0 SDK on your system.
2. Any IDE that supports .NET 9.0 ([vscode](https://code.visualstudio.com/download), [Rider](https://www.jetbrains.com/fr-fr/rider/), [Visual Studio 2022](https://visualstudio.microsoft.com/fr/vs/community/)) 

## Development

1. Fork and clone the repo
   ```sh
   git clone https://github.com/{{ Username }}/Proxarr.git
   cd Proxarr
   ```
  
2. Update your [config.yml](./src/Proxarr.Api/config.yml) with your API keys according to your setup
   ```yaml
    AppConfiguration:
      TMDB_API_KEY: {{ TMDB API KEY}}
      TAG_NAME: "q" # must be the same as the tag name in indexers
      WATCH_PROVIDERS: "FR:Netflix,US:Amazon Prime Video" # format: REGION:PROVIDER,REGION:PROVIDER
      Clients: # one or more Sonarr/Radarr clients
        - ApiKey: {{ SONARR_OR_RADARR_API_KEY }}
          BaseUrl: "https://radarr.com"
        - ApiKey: {{ SONARR_OR_RADARR_API_KEY }}
          BaseUrl: "https://sonarr.com"
   ``` 
   
3. Run the application
   ```sh
   dotnet run ./src/Proxarr.Api/Proxarr.Api.csproj
   ```

<p align="right">
    <a href="#readme-top">
        <img src="images/back-to-top.png" alt="back to top" width="35" />
    </a>
</p>