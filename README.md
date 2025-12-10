# Dizmuze

<div align="center">

![License](https://img.shields.io/badge/license-MIT-green)

</div>

Dizmuze is a lightweight Discord bot that plays audio from a variety of websites via Lavalink. Built with Discord.NET and Lavalink4NET packages.

## Getting started

This document assumes you know how to set up and invite bots to your server. If you don't, refer to the first half of [Discrod.NET's guide](https://docs.discordnet.dev/guides/getting_started/first-bot.html) or check Discord's official documentation.

### Prerequisites

- .NET 9.0 or higher
- Java 17 or higher

### Configuration

Before launching anything, Dizmuze and the Lavalink server need to be configured. To begin with, rename the 'ExampleConfig' file Dizemuze comes with to 'Config' and open it. You will be greeted with the following fields:

- **Discord : Token** - This is where your bot token goes. Without it, your bot won't be able to log in.
- **Lavalink: BaseAddress** - Address and port of the Lavalink server that Dizmuze will connect to. Don't change if you'll be hosting on your own machine.
- **Lavalink: Passphrase** - Password for the Lavalink server.
- **Logging** - How detailed you want the console logging to be. Refer to [Miscrosoft's official documentation](https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=net-10.0-pp) to see which log level names are valid.

If you'll be hosting Lavalink on your machine, go to the Lavalink folder and open 'application.yml' in a text editor. The only setting you need to change there is under 'lavalink:server:password' - make sure it's the same as the Passphrase you set earlier.

### Hosting Lavalink

To host Lavalink locally, all you need to do is run Powershell in the folder Lavalink.jar is located in, and enter the following command: 
```java -jar Lavalink.jar``` 

Then wait until you see the following message: "Lavalink is ready to accept connections." 

You are now ready to launch Dizmuze!

## Commands

Once Dizmuze is up and running, you should see your bot's status change to Online in Discord. From there, you can type the following commands in your server:

- **/play [link]** - Plays audio from a specified website link
- **/stop** - Stops and clears audio playback.

## License

This project is licensed under the [MIT License](LICENSE).
