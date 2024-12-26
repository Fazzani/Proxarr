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
    Simple proxy server that can be used to auto qualify requested item media based on their availability on countries watching providers.
    <br />
    <a href="https://github.com/Fazzani/Proxarr"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/Fazzani/Proxarr">View Demo</a>
    ·
    <a href="https://github.com/Fazzani/Proxarr/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    ·
    <a href="https://github.com/Fazzani/Proxarr/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
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

Proxarr is a simple proxy server that can be used to auto qualify if item media maybe downloaded or not, based on their availability on countries watching providers. 
It is written in .NET (c#).

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- GETTING STARTED -->
## Getting Started

This is an example of how you may give instructions on setting up your project locally.
To get a local copy up and running follow these simple example steps.

### Prerequisites

This is an example of how to list things you need to use the software and how to install them.
* npm
  ```sh
  npm install npm@latest -g
  ```

### Installation

1. Get a free API Key at [https://example.com](https://example.com)
2. Clone the repo
   ```sh
   git clone https://github.com/Fazzani/Proxarr.git
   ```
3. Install NPM packages
   ```sh
   npm install
   ```
4. Enter your API in `config.js`
   ```js
   const API_KEY = 'ENTER YOUR API';
   ```
5. Change git remote url to avoid accidental pushes to base project
   ```sh
   git remote set-url origin Fazzani/Proxarr
   git remote -v # confirm the changes
   ```

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- USAGE EXAMPLES -->
## Usage

Use this space to show useful examples of how a project can be used. Additional screenshots, code examples and demos work well in this space. You may also link to more resources.

```shell
docker run -itd -e LOG_LEVEL=Debug -p 8880:8880 -v ${PWD}/config:/app/config --name proxarr synker/proxarr:latest
```

_For more examples, please refer to the [Documentation](https://example.com)_

<p align="right">(<a href="#readme-top">back to top</a>)</p>

<!-- ROADMAP -->
## Roadmap

- [ ] Improve Docker tagging with git tag
- [ ] Remove secrets from code
- [ ] Add more providers (JustWatch, Reelgood, etc)
- [ ] Health check app/docker
- [ ] Add docker-compose example
- [ ] Add more tests
- [ ] Add more documentation
- [ ] CI/CD pipeline PR
- [ ] Improve logging and error handling
 
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
[product-screenshot]: images/screenshot.png