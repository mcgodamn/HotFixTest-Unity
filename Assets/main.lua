function StaticFunTest()
    CS.UnityEngine.Debug.Log('hello world')
end

function StaticFunTest2(txt)
    CS.UnityEngine.Debug.Log(txt)
end

function AddGenericList()
    local List_String = CS.System.Collections.Generic.List(CS.System.String)
    local lst = List_String()
    lst:Add("123")
end