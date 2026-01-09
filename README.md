 Instalación y Uso 
.

Pasos para ejecutar:
1. Descarga el archivo: Busca el archivo `RecursosHumanos.WinForms.exe` en la raíz de este repositorio (o en la sección de *Releases*).
2. Ejecución: Haz doble clic sobre el archivo `.exe`.
   - *Nota:* Si Windows muestra una advertencia de protección (SmartScreen), haz clic en "Más información" y luego en "Ejecutar de todas formas"
3. *Base de Datos: El sistema utiliza SQLite. La primera vez que abras el programa, se creará automáticamente un archivo llamado `RecursosHumanos.db` en la misma carpeta. Todos tus datos se guardarán localmente en ese archivo.

Tecnologías utilizadas:
- Lenguaje:C# (.NET 8)
- Interfaz: Windows Forms
- Base de Datos: SQLite (Embebida)
- Arquitectura: Diseño orientado a dominios (DDD) con separación en capas (Domain, Infrastructure, Application, WinForms).
- Herramientas:Entity Framework Core, AutoMapper.