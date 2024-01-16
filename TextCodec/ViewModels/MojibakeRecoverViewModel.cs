using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.ViewModels;

public partial class MojibakeRecoverViewModel : ObservableObject
{
    [ObservableProperty]
    private string mojibakeText;
    [ObservableProperty]
    private ObservableCollection<string> recoverTextList = new();

    private readonly List<Encoding> encodings = new();

    public MojibakeRecoverViewModel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        encodings.Add(Encoding.UTF8);
        encodings.Add(Encoding.GetEncoding("gbk"));
        encodings.Add(Encoding.GetEncoding("shift_jis"));
        encodings.Add(Encoding.GetEncoding("big5"));
        encodings.Add(Encoding.GetEncoding("windows-1252"));

        for (int i = 0; i < 25; i++)
        {
            RecoverTextList.Add(string.Empty);
        }
    }

    [RelayCommand]
    private async Task MojibakaRecoverAsync()
    {
        for (int i = 0; i < 5; i++)
        {
            byte[] bytes = await Task.Run(() =>
            {
                return encodings[i].GetBytes(MojibakeText);
            });
            for (int j = 0; j < 5; j++)
            {
                if (i == j) { continue; }
                RecoverTextList[5 * i + j] = await Task.Run(() =>
                {
                    return encodings[j].GetString(bytes);
                });
            }
        }
    }
}
