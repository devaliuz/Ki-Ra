# Ki-Ra - Your Personal Voice Assistant

Ki-Ra is a voice-controlled assistant built on .NET 8 and C#. It utilizes the Vosk speech recognition toolkit and a machine learning model to recognize and process voice commands.

## Features

* **Speech Recognition:** Employs the Vosk speech recognition toolkit to convert spoken language into text.
* **Voice Command Processing:** Interprets recognized voice commands and executes corresponding actions.
* **Text-to-Speech:** Leverages the `System.Speech` library to convert text into spoken language.
* **Background Music:** Plays background music during execution.
* **Database Management:** Stores and manages commands and their corresponding responses in a SQLite database.
* **Extensibility:** New commands and responses can be easily added via voice commands or directly in the database.

## Prerequisites

* .NET 8 SDK
* SQLite

## Installation

1. Clone the repository: 
   ```bash
   git clone https://github.com/devaliuz/Ki-Ra.git

2. Navigate to the project directory:
   ```bash
   -cd Ki-Ra
3. Ensure you have downloaded the Vosk model into the 

    src/Infrastructure/models/lang_Model
    directory.

4. Provide the "appsettings.json" file with the correct "ModelPath".

5. Build the project:
   ```bash
    dotnet build

6. Run the application:
   ```bash
   dotnet run

## Usage
Launch the application.
Wait for the background music to stop and the message "Die App ist jetzt bereit" (The app is now ready) to be spoken.
Speak a command into the microphone.
Ki-Ra will attempt to recognize the command and provide a corresponding response.

## Default Commands
* **"Hallo": Ki-Ra greets you with various responses.
* **"Wie geht es dir?": Ki-Ra provides information about its well-being.
* **"Witz": Ki-Ra tells a joke.
* **"Auf Wiedersehen": Ki-Ra says goodbye.
* **"Neuer Befehl": Allows adding new commands and responses via voice commands.
* **"Was kannst du": Lists all available commands.
