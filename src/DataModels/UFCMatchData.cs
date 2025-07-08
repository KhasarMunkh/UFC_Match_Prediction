// event_name, r_fighter, b_fighter, winner, weight_class, is_title_bout,
// gender, method, finish_round, total_rounds, time_sec, referee, r_kd,
// r_sig_str, r_sig_str_att, r_sig_str_acc, r_str, r_str_att, r_str_acc,
// r_td, r_td_att, r_td_acc, r_sub_att, r_rev, r_ctrl_sec, r_wins_total,
// r_losses_total, r_age, r_height, r_weight, r_reach, r_stance, r_SLpM_total,
// r_SApM_total, r_sig_str_acc_total, r_td_acc_total, r_str_def_total, r_td_def_total,
// r_sub_avg, r_td_avg, b_kd, b_sig_str, b_sig_str_att, b_sig_str_acc, b_str, b_str_att,
// b_str_acc, b_td, b_td_att, b_td_acc, b_sub_att, b_rev, b_ctrl_sec, b_wins_total, b_losses_total,
// b_age, b_height, b_weight, b_reach, b_stance, b_SLpM_total, b_SApM_total, b_sig_str_acc_total, b_td_acc_total,
// b_str_def_total, b_td_def_total, b_sub_avg, b_td_avg, kd_diff, sig_str_diff, sig_str_att_diff, sig_str_acc_diff,
// str_diff, str_att_diff, str_acc_diff, td_diff, td_att_diff, td_acc_diff, sub_att_diff, rev_diff, ctrl_sec_diff, wins_total_diff,
// losses_total_diff, age_diff, height_diff, weight_diff, reach_diff, SLpM_total_diff, SApM_total_diff, sig_str_acc_total_diff,
// td_acc_total_diff, str_def_total_diff, td_def_total_diff, sub_avg_diff, td_avg_diff

// <summary>
// Represents the data model for a UFC match. (Models input data)
using Microsoft.ML.Data;

/// </summary>
namespace MLexperiment.DataModels
{
    public class UFCMatchData
    {
        [LoadColumn(0)] public required string Event_Name { get; set; }
        [LoadColumn(1)] public required string R_fighter { get; set; }
        [LoadColumn(2)] public required string B_fighter { get; set; }
        [LoadColumn(3)] public string Winner { get; set; }
        [LoadColumn(4)] public required string Weight_Class { get; set; }
        [LoadColumn(5)] public bool Is_Title_Bout { get; set; }
        [LoadColumn(6)] public required string Gender { get; set; }

        [LoadColumn(31)] public required string R_Stance { get; set; }
        [LoadColumn(59)] public required string B_Stance { get; set; }
        [LoadColumn(81)] public float Wins_Total_Diff { get; set; }
        [LoadColumn(82)] public float Losses_Total_Diff { get; set; }
        [LoadColumn(83)] public float Age_Diff { get; set; }
        [LoadColumn(84)] public float Height_Diff { get; set; }
        [LoadColumn(85)] public float Weight_Diff { get; set; }
        [LoadColumn(86)] public float Reach_Diff { get; set; }
        [LoadColumn(92)] public float TD_Def_Diff { get; set; }
        [LoadColumn(93)] public float Sub_Diff { get; set; }
        [LoadColumn(94)] public float TD_Diff { get; set; }
    }
}