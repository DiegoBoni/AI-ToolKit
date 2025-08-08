# Toolkit de Integración de OpenAI para Unity (AI-Toolkit)

¡Buenas! Este es un toolkit para integrar la API de OpenAI en tus proyectos de Unity de una manera súper flexible y fácil de manejar, pensado por y para desarrolladores de juegos.

La magia de este asset está en su arquitectura basada en **Scriptable Objects (SO)**. En lugar de hardcodear configuraciones, todo se gestiona a través de assets que podés crear, modificar y asignar directamente desde el editor de Unity.

## Guía Rápida para Empezar

### 1. Configurar la API Key (¡Importante!)

Por seguridad, el toolkit carga tu API Key de OpenAI desde un archivo `.env` que debe estar en la raíz de tu proyecto.

1.  Creá un archivo llamado `.env` en la carpeta principal del proyecto (al mismo nivel que `Assets` y `ProjectSettings`).
2.  Adentro, poné esta línea (cambiando `sk-TuClave` por tu clave secreta):
    ```
    OPENAI_API_KEY=sk-TuClave
    ```
3.  **¡FUNDAMENTAL!** Agregá la línea `.env` a tu archivo `.gitignore` para no exponer tu clave en el repositorio. El toolkit la va a detectar y usar automáticamente.

### 2. Crear un "Cerebro" (Scriptable Object)

-   Hacé clic derecho en la ventana de Project y andá a `Create > AI-Toolkit > Brains`.
-   Elegí el tipo de cerebro que necesitás (ej: `Chat Completion Brain`).
-   Se creará un nuevo asset. Seleccionalo y ajustá sus parámetros en el Inspector.

### 3. Usar el Cerebro en un Script

-   En cualquier script de MonoBehaviour, declará una variable pública del tipo de cerebro que creaste.
    ```csharp
    public OpenAIChatCompletionBrain npcChatBrain;
    ```
-   Guardá y volvé al editor de Unity.
-   Arrastrá el asset del cerebro al campo correspondiente en el Inspector de tu componente.
-   ¡Listo! Ahora desde tu código podés llamar a sus métodos para interactuar con la API.
    ```csharp
    // ¡Esto es un ejemplo, la función real puede variar!
    npcChatBrain.SendRequest("Hola, ¿cómo va todo?");
    ```

## Funcionalidades y Ejemplos

La escena de ejemplo `AI-SampleScene` incluye demostraciones prácticas de todas las funcionalidades clave:

-   **Chat y Completion**: Conversaciones dinámicas con modelos como GPT-4 y GPT-3.5-Turbo.
-   **Asistentes (GPTs)**: Conexión e interacción con agentes Asistentes especializados.
-   **Generación de Imágenes (DALL·E)**: Creación de imágenes a partir de texto.
-   **Texto a Voz (TTS)**: Conversión de texto a audio.
-   **Voz a Texto (STT)**: Transcripción de audio a texto.
-   **Visión**: Análisis y reconocimiento de imágenes capturadas por una cámara.

---
*Hecho con ❤️ por Diego (BoniNerd)*
