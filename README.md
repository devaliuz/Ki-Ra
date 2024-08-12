# Ki-Ra - Your Personal Voice Assistant

Ki-Ra is a voice-controlled assistant built on .NET 8 and C#. It utilizes the Vosk speech recognition toolkit and a machine learning model to recognize and process voice commands.

## Features

* **Speech Recognition:** Employs the Vosk speech recognition toolkit to convert spoken language into text.
* **Voice Command Processing:** Interprets recognized voice commands and executes corresponding actions.
* **Text-to-Speech:** Leverages the `System.Speech` library to convert text into spoken language.
* **Background Music:** Plays background music during initialization and execution.
* **Database Management:** Stores and manages commands, their corresponding responses, and synonyms for commands in a SQLite database.
* **Extensibility:** New commands and responses can be easily added via voice commands or directly in the database.
* **Trigger word activation**: Ki-Ra will only listen for commands after the trigger word "Kira" is spoken.
* **English support**: UI and "Default-Commands" support english.


## Prerequisites

* .NET 8 SDK
* SQLite
* Vosk model files 

## Installation

1. Clone the repository: 
   ```bash
   git clone [https://github.com/devaliuz/Ki-Ra.git](https://github.com/devaliuz/Ki-Ra.git)
   
2. Navigate to the project directory:

   ```bash
   cd Ki-Ra

3. Download your preferred Vosk model from:

   [https://alphacephei.com/vosk/models](https://alphacephei.com/vosk/models)

4. Place the downloaded model files into the src/Infrastructure/models/lang_Model directory.

5. Update the appsettings.json file with the correct ModelPath pointing to the directory where you placed the model files

6. Build the project:

   ```bash
   dotnet build

7. Run the application:

   ```bash
   dotnet run

## Usage

1. Launch the application.
2. Wait for the background music to stop.
3. Say the trigger word "Kira"
4. Speak a command into the microphone.
5. Ki-Ra will attempt to recognize the command and provide a corresponding response.

## Default Commands

* **"Hello":** Ki-Ra greets you with various responses.
* **"Wow are you?":** Ki-Ra provides information about its well-being.
* **"Tell a Joke":** Ki-Ra tells a joke.
* **"Good Bye":** Ki-Ra says goodbye.
* **"Help":** Lists all available commands.
* **"Befehle verwalten":** Allows you to add, delete or modify commands and their responses

## Notes

* Ki-Ra uses a machine learning model for speech recognition, so accuracy may vary depending on the quality of your microphone and the clarity of your speech
* The application will prompt you to select a Vosk language model if multiple models are found in the ModelPath directory
* You can add new commands and responses directly to the database if you prefer.
* Ki-Ra supports synonyms for commands, allowing for more flexible voice interaction
