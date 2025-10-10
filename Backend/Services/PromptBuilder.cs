public class PromptBuilder
{
    public static string Build(string historySummary, string availableSummary)
    {
        //Texten som skickas till AI 
        //$@Interpolerad sträng där jag slipper escapea tecken och kan skriva på flera rader, utan \n
        return $@"
        Du är en bokningsassistans.
        Analysera användarens bokningshistorik och rekommendera en resurs + tid för en kommande dag.

        Regler:
        - Du får ENDAST välja resurser och tider som finns i listan nedan.
        - Du får ENDAST välja datum från idag och framåt.
        - Du får INTE hitta på resursnamn eller tider.
        - Returnera *endast ren JSON utan ```json-markeringar* eller förklaringar.
        - Returnera endast giltig JSON enligt detta schema:
        {{
        ""ResourceType"": ""string"",
        ""ResourceName"": ""string"",
        ""ResourceTypeId"":int,
        ""Date"": ""yyyy-MM-dd"",
        ""TimeSlot"": ""HH-HH""
        }}

        Användarhistorik:
        {historySummary}

        Lediga resurser och tider:
        {availableSummary}
        ";
    }
}