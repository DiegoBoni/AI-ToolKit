# Unity OpenAI Integration Toolkit (AI-Toolkit)

A powerful and flexible toolkit for integrating the OpenAI API into your Unity projects. Designed with a developer-first approach, this asset simplifies complex API interactions into manageable, modular components.

The core of this toolkit is its architecture based on **Scriptable Objects (SOs)**. Instead of hardcoding configurations or dealing with complex manager classes, you manage everything through assets that you can create, modify, and assign directly from the Unity editor.

## Quick Start Guide

### 1. API Key Setup (Important!)

For security, this toolkit is designed to load your OpenAI API Key from a `.env` file at the root of your Unity project.

1.  Create a file named `.env` in your project's root directory (the same level as your `Assets` and `ProjectSettings` folders).
2.  Inside the `.env` file, add the following line, replacing `sk-YourKey` with your actual secret key:
    ```
    OPENAI_API_KEY=sk-YourKey
    ```
3.  **CRUCIAL**: Add `.env` to your `.gitignore` file to prevent your key from being exposed in version control. The toolkit will automatically detect and use this key.

### 2. Create a "Brain" (Scriptable Object)

-   Right-click in the Project window, navigate to `Create > AI-Toolkit > Brains`.
-   Choose the type of brain you need (e.g., `Chat Completion Brain`).
-   A new asset will be created. Select it and configure its parameters in the Inspector.

### 3. Use the Brain in a Script

-   In any MonoBehaviour script, declare a public variable for the brain you created.
    ```csharp
    public OpenAIChatCompletionBrain npcChatBrain;
    ```
-   Save the script and return to the Unity editor.
-   Drag the brain asset you created in Step 2 onto the corresponding field in your component's Inspector.
-   Now you can call its methods from your code to interact with the OpenAI API.
    ```csharp
    // Example method call, actual function may vary!
    npcChatBrain.SendRequest("Hello, how are you?");
    ```

## Key Features & Samples

The included `AI-SampleScene` provides practical, ready-to-use examples for all major features:

-   **Chat & Completion**: Engage in dynamic conversations with models like GPT-4 and GPT-3.5-Turbo.
-   **Assistants (GPTs)**: Connect to and interact with specialized Assistant agents.
-   **Image Generation (DALL·E)**: Generate images from text prompts.
-   **Text-to-Speech (TTS)**: Convert text into spoken audio.
-   **Speech-to-Text (STT)**: Transcribe audio into text.
-   **Vision**: Analyze and understand images captured from a camera.

---
*Made with ❤️ by Diego (BoniNerd)*
