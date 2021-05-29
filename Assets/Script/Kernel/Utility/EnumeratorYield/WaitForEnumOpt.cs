using UnityEngine;

public class WaitForEnumOpt : CustomYieldInstruction
{
    protected IEnumOpt mEnumOpt;
    public WaitForEnumOpt(IEnumOpt eo)
    {
        mEnumOpt = eo;
    }
    public override bool keepWaiting
    {
        get
        {
            return !mEnumOpt.Finished;
        }
    }

}
