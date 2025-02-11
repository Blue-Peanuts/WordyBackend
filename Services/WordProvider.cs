namespace WordyBackend.Services;

public class WordProvider
{
    private List<string> _words = [];
    public WordProvider()
    {
        LoadFromFile(Path.Combine(Directory.GetCurrentDirectory(), "Data", "words.txt"));
    }

    public WordProvider(string filePath)
    {
        LoadFromFile(filePath);
    }
    
    public void LoadFromFile(string filePath)
    {
        if (File.Exists(filePath))
            _words = File.ReadAllLines(filePath)
                .Where(w => !string.IsNullOrWhiteSpace(w))
                .Select(w => w.Trim().ToLower())
                .ToList();
        else
            throw new FileNotFoundException("File not found", filePath);
    }
    
    public string GetRandomWord()
    {
        return _words[new Random().Next(0, _words.Count)];
    }
    
    public string GetRandomWord(int length)
    {
        var words = _words.Where(w => w.Length == length).ToList();
        return words[new Random().Next(0, words.Count)];
    }
}