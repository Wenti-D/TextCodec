using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace TextCodec.ViewModels;

public partial class MojibakeRecoverViewModel : ObservableObject
{
    [ObservableProperty]
    private string mojibakeText;
    [ObservableProperty]
    private List<string> recoveredTextList;
    

    private readonly Encoding[] encodings;

    public MojibakeRecoverViewModel()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        encodings = [
            Encoding.UTF8,
            Encoding.GetEncoding("gbk"),
            Encoding.GetEncoding("shift_jis"),
            Encoding.GetEncoding("big5"),
            Encoding.GetEncoding("windows-1252"),
        ];

        RecoveredTextList = [];
        for (int i = 0; i < 25; i++)
        {
            RecoveredTextList.Add(string.Empty);
        }
    }

    [RelayCommand]
    private async Task MojibakaRecoverAsync()
    {
        string[] recoverTextList = new string[25];
        await Task.Run(() =>
        {
            try
            {
                Parallel.For(0, 5, (i) =>
                {
                    byte[] bytes = encodings[i].GetBytes(MojibakeText);
                    Parallel.For(0, 5, (j) =>
                    {
                        if (i != j)
                        {
                            recoverTextList[5 * i + j] = encodings[j].GetString(bytes);
                        }
                    });
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString() + ex.Message);
            }
        });
        RecoveredTextList = [.. recoverTextList];
    }
}
