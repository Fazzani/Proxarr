<a id="readme-top"></a>
[![Contributors][contributors-shield]][contributors-url]
[![Forks][forks-shield]][forks-url]
[![Stargazers][stars-shield]][stars-url]
[![Issues][issues-shield]][issues-url]
[![project_license][license-shield]][license-url]
[![LinkedIn][linkedin-shield]][linkedin-url]

[![Docker Image CI](https://github.com/Fazzani/Proxarr/actions/workflows/docker-image.yml/badge.svg)](https://github.com/Fazzani/Proxarr/actions/workflows/docker-image.yml)

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/Fazzani/Proxarr">
    <img src="images/logo.png" alt="Logo" width="320" height="320">
  </a>

  <p align="center">
    Automatically categorize your requested movies and TV shows from your watching providers.
    <br />
    <a href="https://github.com/Fazzani/Proxarr/issues/new?labels=bug&template=bug-report.yml">Report Bug</a>
    ·
    <a href="https://github.com/Fazzani/Proxarr/issues/new?labels=enhancement&template=feature-request.yml">Request Feature</a>
  </p>
</div>

<!-- TABLE OF CONTENTS -->
<details>
  <summary>Table of Contents</summary>
  <ol>
    <li>
      <a href="#about-the-project">About The Project</a>
    </li>
    <li>
      <a href="#getting-started">Getting Started</a>
      <ul>
        <li><a href="#prerequisites">Prerequisites</a></li>
        <li><a href="#installation">Installation</a></li>
      </ul>
    </li>
    <li><a href="#usage">Usage</a></li>
    <li><a href="#roadmap">Roadmap</a></li>
    <li><a href="#contributing">Contributing</a></li>
    <li><a href="#license">License</a></li>
    <li><a href="#contact">Contact</a></li>
  </ol>
</details>

<!-- ABOUT THE PROJECT -->
## About The Project

Proxarr is a lightweight proxy server for automatically qualify requested media items based in countries served by watching providers.
It uses TMDB to find out which streaming services are available in the selected region.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

### Prerequisites

* Acquire TMDB API KEY
  [How](https://dev.to/codexive_zech/streamlining-your-contribution-how-to-get-your-tmdb-api-key-for-ldbflix-contribution-52gf#:~:text=How%20to%20Obtain%20a%20TMDB%20API%20Key)
* Obtain SONARR/RADARR API KEY<br/>
  <img src="images/arr_api_key.png" width="230">

### Installation

1. Clone the repo
   ```sh
   git clone https://github.com/Fazzani/Proxarr.git
   ```
  
2. Update your [config.yml](./src/Proxarr.Api/config.yml) with your API keys according to your setup
   ```yaml
    AppConfiguration:
        TMDB_API_KEY: "{{ TMDB API KEY}}"
        TAG_NAME: "q"
        WATCH_PROVIDERS: "YOUR PROVIDERS HERE seperated by comma (ex: FR:Netflix,US:Amazon Prime Video)"
        Clients: # Add as many clients as you want
        - ApiKey: "{{ SONAR OR RADARR API KEY }}"
          BaseUrl: "{{ SONARR OR RADARR BASE URL }}"
   ``` 
   
3. Run the application
   ```sh
   dotnet run ./src/Proxarr.Api/Proxarr.Api.csproj
   ```
<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

### Standalone docker container example

```shell
docker run -itd --rm -e LOG_LEVEL=Debug -p 8880:8880 -v ${PWD}/config:/app/config --name proxarr synker/proxarr:latest
```
### Full example with docker compose

```shell
docker compose -f docker-compose.yml up -d
```
<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Watching providers configuration

- [TMDB API to get available regions](https://developer.themoviedb.org/reference/watch-providers-available-regions)
- [TMDB API to get available movie providers by region](https://developer.themoviedb.org/reference/watch-providers-movie-list)
- [TMDB API to get available tv providers by region](https://developer.themoviedb.org/reference/watch-providers-tv-list)

<!-- ROADMAP -->
## Roadmap

- [ ] Remove secrets from code
- [ ] Add more providers (JustWatch, Reelgood, etc)
- [ ] Add more tests
- [ ] Add more documentation
- [ ] CI/CD pipeline PR
- [ ] Improve logging and error handling
- [ ] Full scan library
- [ ] Api versioning
 
See the [open issues](https://github.com/Fazzani/Proxarr/issues) for a full list of proposed features (and known issues).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTRIBUTING -->
## Contributing

Contributions are what make the open source community such an amazing place to learn, inspire, and create. Any contributions you make are **greatly appreciated**.

If you have a suggestion that would make this better, please fork the repo and create a pull request. You can also simply open an issue with the tag "enhancement".
Don't forget to give the project a star! Thanks again!

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

<p align="right">(<a href="#readme-top">back to top</a>)</p>

### Top contributors:

<a href="https://github.com/Fazzani/Proxarr/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=Fazzani/Proxarr" alt="contrib.rocks image" />
</a>

<!-- LICENSE -->
## License

Distributed under the project_license. See `LICENSE.txt` for more information.

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- CONTACT -->
## Contact

Fazzani - heni.fazzani@gmail.com

Project Link: [https://github.com/Fazzani/Proxarr](https://github.com/Fazzani/Proxarr)

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- MARKDOWN LINKS & IMAGES -->
[contributors-shield]: https://img.shields.io/github/contributors/Fazzani/Proxarr.svg?style=for-the-badge
[contributors-url]: https://github.com/Fazzani/Proxarr/graphs/contributors
[forks-shield]: https://img.shields.io/github/forks/Fazzani/Proxarr.svg?style=for-the-badge
[forks-url]: https://github.com/Fazzani/Proxarr/network/members
[stars-shield]: https://img.shields.io/github/stars/Fazzani/Proxarr.svg?style=for-the-badge
[stars-url]: https://github.com/Fazzani/Proxarr/stargazers
[issues-shield]: https://img.shields.io/github/issues/Fazzani/Proxarr.svg?style=for-the-badge
[issues-url]: https://github.com/Fazzani/Proxarr/issues
[license-shield]: https://img.shields.io/github/license/Fazzani/Proxarr.svg?style=for-the-badge
[license-url]: https://github.com/Fazzani/Proxarr/blob/master/LICENSE.txt
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://www.linkedin.com/in/heni-fazzani
[arr-api-key]: images/arr_api_key.png
