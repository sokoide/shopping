public class Consts
{
    public const string OTEL_SERVICE_NAME = "Nintenbo-backend";
    public const string OTLP_GRPC_ENDPOINT = "http://localhost:4317/";
    public const string OTLP_HTTP_ENDPOINT = "http://localhost:4318/";
    public static string[] CORS_ORIGINS = new[] { $"http://{Globals.GetLocalIPAddress()}:3000", "http://localhost:3000" };
    public const int WEBAPI_PORT = 15001;
    // private const string ImageBaseUrl = "http://localhost:15001/";
    private static string ImageBaseUrl = $"http://{Globals.GetLocalIPAddress()}:15001/";


    private static Product[] products = new[] {
        new Product(1, "Mario", 10.0, "Cheerful, mustached Italian plumber known for his red hat and blue overalls, who embarks on adventurous quests to rescue Princess Peach from various villains, primarily Bowser.", "Mario's family", ImageBaseUrl + "static/Slice 1.png"),
        new Product(2, "Luigi", 10.0, "Mario's taller, younger brother, known for his green hat and overalls, who often accompanies Mario on adventures, displaying bravery despite his more timid and cautious nature.", "Mario's family", ImageBaseUrl + "static/Slice 2.png"),
        new Product(3, "Peach", 15.0, "Kind-hearted and regal ruler of the Mushroom Kingdom, often wearing a pink dress and crown, who frequently needs rescuing from villains like Bowser but also showcases her own strength and leadership in various adventures.", "Mario's family", ImageBaseUrl + "static/Slice 3.png"),
        new Product(4, "Daisy", 15.0, "Spirited and athletic ruler of Sarasaland, known for her orange dress and energetic personality, often ", "Mario's family", ImageBaseUrl + "static/Slice 4.png"),
        new Product(5, "Rosalina", 20.0, "Wise and enigmatic protector of the cosmos, often seen in her light blue gown, who watches over the Lumas from the Comet Observatory and aids Mario in his galaxy-spanning adventures.", "Mario's family", ImageBaseUrl + "static/Slice 5.png"),
        new Product(6, "Yoshi", 10.0, "Friendly and loyal dinosaur with a long tongue and a distinctive green appearance, known for his ability to eat enemies, lay eggs, and carry Mario on his back during their adventures.", "Mario's family", ImageBaseUrl + "static/Slice 6.png"),
        new Product(7, "Wario", 1.50, "Greedy, mischievous anti-hero with a stout build, yellow and purple attire, and a distinctive zigzag mustache, often engaging in schemes to amass wealth and challenge Mario.", "Wario's family", ImageBaseUrl + "static/Slice 7.png"),
        new Product(8, "Waluigi", 1.25, "Lanky, cunning partner in mischief, characterized by his purple attire, long limbs, and a thin, curled mustache, often seen causing trouble and competing against Mario and Luigi in various games.", "Wario's family", ImageBaseUrl + "static/Slice 8.png"),
        new Product(9, "Donky Kong", 10.50, "Strong, barrel-throwing gorilla with a red tie, known for his adventures in the jungle, often protecting his home from various villains and helping his friends, including Diddy Kong.", "Donky's family", ImageBaseUrl + "static/Slice 9.png"),
        new Product(10, "Diddy Kong", 10.50, "Donkey Kong's energetic and agile sidekick, recognizable by his red cap and shirt, known for his quick reflexes and playful nature as he assists Donkey Kong on their jungle adventures.", "Donky's family", ImageBaseUrl + "static/Slice 10.png"),
        new Product(11, "Hammer Bro", 9.50, "Formidable, helmet-wearing Koopa Troopa known for his ability to throw hammers with precision, often appearing as a challenging enemy in Mario's adventures.  ", "Bowser's family", ImageBaseUrl + "static/Slice 11.png"),
        new Product(12, "Hammer Bro Red", 9.50, "Variant of the standard Hammer Bro, distinguished by his red shell and helmet, who also throws hammers with precision and presents a tougher challenge in Mario's adventures.", "Bowser's family", ImageBaseUrl + "static/Slice 12.png"),
        new Product(13, "Bowser", 9.50, "Powerful and fearsome King of the Koopas, characterized by his spiked shell, fiery breath, and relentless pursuit of Princess Peach, often serving as Mario's primary antagonist.", "Bowser's family", ImageBaseUrl + "static/Slice 13.png"),
        new Product(14, "Bowser Jr", 9.50, "Bowser's mischievous and crafty son, recognizable by his bib resembling his father's mouth, who often aids in his father's schemes to capture Princess Peach and thwart Mario.", "Bowser's family", ImageBaseUrl + "static/Slice 14.png"),
        new Product(15, "Boo", 9.50, "Ghostly enemy known for its shy behavior, covering its face when looked at directly, and its mischievous nature, often haunting Mario and his friends in spooky environments.", "Bowser's family", ImageBaseUrl + "static/Slice 15.png"),
        new Product(16, "Goomba", 9.50, "Small, brown, mushroom-like enemy with a frowning face and tiny feet, known for being one of the most common and easily defeated adversaries in Mario's adventures.", "Bowser's family", ImageBaseUrl + "static/Slice 16.png"),
        new Product(17, "Maskass", 9.50, "Also known as Shy Guy, is a small, masked character with a simple robe and a timid demeanor, often seen working for Bowser or other villains and causing trouble for Mario and his friends.", "Bowser's family", ImageBaseUrl + "static/Slice 17.png"),
        new Product(18, "Dry Bones", 9.50, "Skeletal Koopa Troopa, often found in castles and haunted environments, known for collapsing into a pile of bones when defeated and reassembling itself shortly afterward to continue pursuing Mario.", "Bowser's family", ImageBaseUrl + "static/Slice 18.png"),
    };

    public static Product[] GetProducts()
    {
        return products;
    }

    public static string GetProductName(string id)
    {
        int iid;
        if (int.TryParse(id, out iid))
        {
            return GetProductName(iid);
        }

        return "";
    }
    public static string GetProductName(int id)
    {
        foreach (var product in products)
        {
            if (product.ID == id)
            {
                return product.Title;
            }
        }

        return "";
    }
}
