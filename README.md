# Levantar Proyecto Item.Work.Api

1. La base de datos utilizada para el proyecto es SQL SERVER
2. Solo habria el conection string en el archivo appsettings.json por defecto el nombre de la base de datos para almacenar toda la informacion se llama **DbItemsWork**
3. Si al momento de levantar el api de User se tendria el cambiar la url que se encuentra en el appsettings.json en la seccion de Services -> UserService actualmente esta aputando a https://localhost:7035/api/user/
4. Despues correr la migracion
    1. Package manager: **Update-Database**
    2. Línea de comandos: **dotnet ef database update**
5. Con toda estas revisiones no deberia tener problemas