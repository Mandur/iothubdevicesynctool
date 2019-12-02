# iothubdevicesynctool

This tool migrate devices from an IoT Hub to another one. It i based on the [following documentation](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-bulk-identity-mgmt).

# Quickstart
1. Create/use an existing storage account
2. Create a shared access policies on the storage account blade on Azure and copy the storage account in appsettings.json. Append a blob name to the existring url
3. Provision the iothub owner connection string in the appsettings.json
4. Run the script