using EbookService.Models;

namespace EbookService.Services;

public class InMemoryBookCatalogService : IBookCatalogService
{
    private readonly List<Book> _books = CreateBooks();

    public IQueryable<Book> GetBooks() => _books.AsQueryable();

    public Book? GetBookById(int id) => _books.FirstOrDefault(book => book.Id == id);

    private static List<Book> CreateBooks()
    {
        var books = new List<Book>();
        var id = 1;

        AddGenreBooks(books, ref id, "Romance",
        [
            new BookSeed("Eugene Onegin", "Alexander Pushkin", 1833, "Russian", 11.90m),
            new BookSeed("The Lady with the Dog", "Anton Chekhov", 1899, "Russian", 8.60m),
            new BookSeed("Wuthering Heights", "Emily Bronte", 1847, "English", 10.80m),
            new BookSeed("Sense and Sensibility", "Jane Austen", 1811, "English", 10.40m),
            new BookSeed("Anna Karenina", "Leo Tolstoy", 1878, "Russian", 13.20m),
            new BookSeed("The Notebook", "Nicholas Sparks", 1996, "English", 9.90m),
            new BookSeed("Me Before You", "Jojo Moyes", 2012, "English", 10.95m),
            new BookSeed("Outlander", "Diana Gabaldon", 1991, "English", 12.10m),
            new BookSeed("The Time Traveler's Wife", "Audrey Niffenegger", 2003, "English", 11.20m),
            new BookSeed("Love in the Time of Cholera", "Gabriel Garcia Marquez", 1985, "Spanish", 12.70m)
        ]);

        AddGenreBooks(books, ref id, "Science Fiction",
        [
            new BookSeed("Roadside Picnic", "Arkady and Boris Strugatsky", 1972, "Russian", 12.80m),
            new BookSeed("We", "Yevgeny Zamyatin", 1924, "Russian", 10.20m),
            new BookSeed("Neuromancer", "William Gibson", 1984, "English", 12.40m),
            new BookSeed("Snow Crash", "Neal Stephenson", 1992, "English", 11.85m),
            new BookSeed("The Left Hand of Darkness", "Ursula K. Le Guin", 1969, "English", 12.30m),
            new BookSeed("Ender's Game", "Orson Scott Card", 1985, "English", 10.95m),
            new BookSeed("The Martian", "Andy Weir", 2011, "English", 12.60m),
            new BookSeed("Hyperion", "Dan Simmons", 1989, "English", 13.10m),
            new BookSeed("Fahrenheit 451", "Ray Bradbury", 1953, "English", 9.80m),
            new BookSeed("Do Androids Dream of Electric Sheep?", "Philip K. Dick", 1968, "English", 11.40m)
        ]);

        AddGenreBooks(books, ref id, "Fantasy",
        [
            new BookSeed("Ruslan and Ludmila", "Alexander Pushkin", 1820, "Russian", 9.80m),
            new BookSeed("The Night Watch", "Sergei Lukyanenko", 1998, "Russian", 11.70m),
            new BookSeed("The Two Towers", "J.R.R. Tolkien", 1954, "English", 12.95m),
            new BookSeed("The Return of the King", "J.R.R. Tolkien", 1955, "English", 12.95m),
            new BookSeed("A Wizard of Earthsea", "Ursula K. Le Guin", 1968, "English", 10.90m),
            new BookSeed("The Name of the Wind", "Patrick Rothfuss", 2007, "English", 14.20m),
            new BookSeed("The Lies of Locke Lamora", "Scott Lynch", 2006, "English", 12.80m),
            new BookSeed("The Last Unicorn", "Peter S. Beagle", 1968, "English", 10.40m),
            new BookSeed("Mistborn: The Final Empire", "Brandon Sanderson", 2006, "English", 13.30m),
            new BookSeed("The Eye of the World", "Robert Jordan", 1990, "English", 11.90m)
        ]);

        AddGenreBooks(books, ref id, "Thriller",
        [
            new BookSeed("Metro 2033", "Dmitry Glukhovsky", 2005, "Russian", 12.40m),
            new BookSeed("The Death of Achilles", "Boris Akunin", 1998, "Russian", 10.90m),
            new BookSeed("The Da Vinci Code", "Dan Brown", 2003, "English", 10.60m),
            new BookSeed("The Silent Patient", "Alex Michaelides", 2019, "English", 11.80m),
            new BookSeed("The Bourne Identity", "Robert Ludlum", 1980, "English", 9.95m),
            new BookSeed("Shutter Island", "Dennis Lehane", 2003, "English", 10.85m),
            new BookSeed("Before I Go to Sleep", "S.J. Watson", 2011, "English", 9.70m),
            new BookSeed("The Woman in the Window", "A.J. Finn", 2018, "English", 11.15m),
            new BookSeed("I Am Pilgrim", "Terry Hayes", 2013, "English", 12.40m),
            new BookSeed("The Reversal", "Michael Connelly", 2010, "English", 10.20m)
        ]);

        AddGenreBooks(books, ref id, "Mystery",
        [
            new BookSeed("The Winter Queen", "Boris Akunin", 1998, "Russian", 10.30m),
            new BookSeed("Murder on the Leviathan", "Boris Akunin", 1998, "Russian", 10.20m),
            new BookSeed("The Maltese Falcon", "Dashiell Hammett", 1930, "English", 9.40m),
            new BookSeed("The Big Sleep", "Raymond Chandler", 1939, "English", 9.35m),
            new BookSeed("The Moonstone", "Wilkie Collins", 1868, "English", 8.90m),
            new BookSeed("And Then There Were None", "Agatha Christie", 1939, "English", 10.10m),
            new BookSeed("In the Woods", "Tana French", 2007, "English", 10.80m),
            new BookSeed("The No. 1 Ladies' Detective Agency", "Alexander McCall Smith", 1998, "English", 9.60m),
            new BookSeed("The Cuckoo's Calling", "Robert Galbraith", 2013, "English", 10.95m),
            new BookSeed("Still Life", "Louise Penny", 2005, "English", 10.50m)
        ]);

        AddGenreBooks(books, ref id, "Horror",
        [
            new BookSeed("Viy", "Nikolai Gogol", 1835, "Russian", 8.40m),
            new BookSeed("The Family of the Vourdalak", "Alexei Tolstoy", 1840, "Russian", 8.95m),
            new BookSeed("Frankenstein", "Mary Shelley", 1818, "English", 8.95m),
            new BookSeed("The Haunting of Hill House", "Shirley Jackson", 1959, "English", 9.65m),
            new BookSeed("Pet Sematary", "Stephen King", 1983, "English", 10.25m),
            new BookSeed("Bird Box", "Josh Malerman", 2014, "English", 9.90m),
            new BookSeed("The Exorcist", "William Peter Blatty", 1971, "English", 9.70m),
            new BookSeed("It", "Stephen King", 1986, "English", 12.20m),
            new BookSeed("Mexican Gothic", "Silvia Moreno-Garcia", 2020, "English", 10.60m),
            new BookSeed("The Turn of the Screw", "Henry James", 1898, "English", 8.85m)
        ]);

        AddGenreBooks(books, ref id, "Biography",
        [
            new BookSeed("Childhood", "Maxim Gorky", 1913, "Russian", 8.80m),
            new BookSeed("The Story of My Contemporary", "Vladimir Korolenko", 1909, "Russian", 9.10m),
            new BookSeed("The Story of My Life", "Helen Keller", 1903, "English", 9.35m),
            new BookSeed("Steve Jobs", "Walter Isaacson", 2011, "English", 13.10m),
            new BookSeed("Alexander Hamilton", "Ron Chernow", 2004, "English", 14.60m),
            new BookSeed("Churchill: A Life", "Martin Gilbert", 1991, "English", 12.30m),
            new BookSeed("Einstein: His Life and Universe", "Walter Isaacson", 2007, "English", 13.40m),
            new BookSeed("Team of Rivals", "Doris Kearns Goodwin", 2005, "English", 13.25m),
            new BookSeed("The Wright Brothers", "David McCullough", 2015, "English", 10.90m),
            new BookSeed("Becoming", "Michelle Obama", 2018, "English", 11.95m)
        ]);

        AddGenreBooks(books, ref id, "Epic Poetry",
        [
            new BookSeed("The Bronze Horseman", "Alexander Pushkin", 1833, "Russian", 9.20m),
            new BookSeed("Poltava", "Alexander Pushkin", 1829, "Russian", 8.90m),
            new BookSeed("The Aeneid", "Virgil", -19, "Latin", 9.95m),
            new BookSeed("The Epic of Gilgamesh", "Anonymous", -2100, "Akkadian", 11.20m),
            new BookSeed("Beowulf", "Anonymous", 975, "Old English", 9.85m),
            new BookSeed("The Divine Comedy", "Dante Alighieri", 1320, "Italian", 12.15m),
            new BookSeed("Paradise Lost", "John Milton", 1667, "English", 10.75m),
            new BookSeed("Mahabharata", "Vyasa", -400, "Sanskrit", 13.50m),
            new BookSeed("Ramayana", "Valmiki", -500, "Sanskrit", 12.80m),
            new BookSeed("Shahnameh", "Ferdowsi", 1010, "Persian", 11.60m)
        ]);

        AddGenreBooks(books, ref id, "Philosophy",
        [
            new BookSeed("The Kingdom of God Is Within You", "Leo Tolstoy", 1894, "Russian", 10.40m),
            new BookSeed("The Philosophy of Freedom", "Nikolai Berdyaev", 1911, "Russian", 11.20m),
            new BookSeed("Nicomachean Ethics", "Aristotle", -340, "Greek", 9.10m),
            new BookSeed("Beyond Good and Evil", "Friedrich Nietzsche", 1886, "German", 9.85m),
            new BookSeed("The Prince", "Niccolo Machiavelli", 1532, "Italian", 8.70m),
            new BookSeed("Being and Time", "Martin Heidegger", 1927, "German", 12.40m),
            new BookSeed("Critique of Pure Reason", "Immanuel Kant", 1781, "German", 12.60m),
            new BookSeed("The Myth of Sisyphus", "Albert Camus", 1942, "French", 10.35m),
            new BookSeed("Tao Te Ching", "Laozi", -400, "Chinese", 8.95m),
            new BookSeed("Thus Spoke Zarathustra", "Friedrich Nietzsche", 1883, "German", 9.75m)
        ]);

        AddGenreBooks(books, ref id, "Historical Fiction",
        [
            new BookSeed("Doctor Zhivago", "Boris Pasternak", 1957, "Russian", 12.50m),
            new BookSeed("And Quiet Flows the Don", "Mikhail Sholokhov", 1940, "Russian", 12.10m),
            new BookSeed("Wolf Hall", "Hilary Mantel", 2009, "English", 12.10m),
            new BookSeed("I, Claudius", "Robert Graves", 1934, "English", 10.50m),
            new BookSeed("The Pillars of the Earth", "Ken Follett", 1989, "English", 12.95m),
            new BookSeed("The Name of the Rose", "Umberto Eco", 1980, "Italian", 11.70m),
            new BookSeed("A Tale of Two Cities", "Charles Dickens", 1859, "English", 9.20m),
            new BookSeed("War and Peace", "Leo Tolstoy", 1869, "Russian", 13.60m),
            new BookSeed("The Other Boleyn Girl", "Philippa Gregory", 2001, "English", 10.40m),
            new BookSeed("The Alice Network", "Kate Quinn", 2017, "English", 11.30m)
        ]);

        return books;
    }

    private static void AddGenreBooks(List<Book> books, ref int id, string genre, IReadOnlyList<BookSeed> seeds)
    {
        foreach (var seed in seeds)
        {
            books.Add(new Book
            {
                Id = id++,
                Title = seed.Title,
                Author = seed.Author,
                Genre = genre,
                Price = seed.Price,
                PublishYear = seed.PublishYear,
                Language = seed.Language
            });
        }
    }

    private readonly record struct BookSeed(string Title, string Author, int PublishYear, string Language, decimal Price);
}
