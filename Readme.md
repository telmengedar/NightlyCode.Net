# NightlyCode.Net

This library contains several helper classes which are useful when creating applications which interact with a network.

## Dependencies

- [HtmlAgilityPack](https://github.com/zzzprojects/html-agility-pack) for Browser interaction / Crawlers

## Browser

The namespace NightlyCode.Net.Browser contains a class SlimBrowser which can be used to write crawlers. It automates redirection and cookie storage so the application can concentrate on loading webpages and analysis of the results.

## Http

The namespace NightlyCode.Net.Http contains multiple implementations of a http server.

### Http.Sys

HttpSysServer is an abstract class which wraps the HttpListener to allow an application to setup a http server and respond to requests easily. Since http.sys usually requires elevated user permissions, using this class shares these requirements.

### Http Server

HttpServer implements a basic http server which focuses on an http server implementation without too much overhead. The usage of the server is still a little bit complex.

## Services

ServiceServer is a basic implementation of a HttpSysServer which allows for service interfaces to be added which respond to http requests.

## TCP

The NightlyCode.Net.TCP namespace contains several TCP helper methods like a netstat wrapper, a raw tcp data proxy and a raw tcp server.