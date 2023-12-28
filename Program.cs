// See https://aka.ms/new-console-template for more information

void ReadDataIn(string textFilePath)
{
    string path = @"E:\Downloads";
    string[] files = Directory.GetFiles(path);
    foreach (string file in files) {
        Console.WriteLine(file);
    }
}

void ProcessData()
{
    
}

ReadDataIn("");