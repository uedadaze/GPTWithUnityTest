コードについては「Editor」フォルダ内をご参照下さい。

# GPT-4とのやりとり

（「AskGPT4FromUnityEditor」のやりとりから続いてきます）

**筆者:**
どうもありがとうございます。無事に意図した通りの動作になりました！

ところで話題は変わるのですが、Unityにおいてstringに入れた文字列をスクリプトとして実行するとしたら、どういう方法があるでしょう？

**GPT-4:**
Unityでは、文字列をスクリプトとして実行するために、以下の方法があります。

1. UnityのC#スクリプトを動的にコンパイルする:
CSharpCodeProviderを使用してC#コードを動的にコンパイルし、生成されたアセンブリを実行します。ただし、CSharpCodeProviderは.NET Frameworkに依存しており、Unityの.NET Standard 2.0や2.1の環境ではデフォルトでは使用できません。そのため、この方法は非推奨です。

2. System.Reflection.Emitを使用する:
System.Reflection.Emit名前空間を使用して、動的にCIL（Common Intermediate Language）コードを生成し、実行します。これにより、実行時にメソッドを動的にコンパイルし、実行することができます。ただし、この方法は複雑であり、UnityのIL2CPP（Intermediate Language to C++）ビルドターゲットと互換性がないため、注意が必要です。

3. ルーチンを実行するための独自のスクリプト言語を実装する:
独自のスクリプト言語を実装し、文字列を解析して該当するC#メソッドや関数を呼び出すことができます。これは、Unity内で実行する文字列に制約を設けることができるため、セキュリティの観点からも優れています。ただし、この方法は言語のパーサーとインタプリタを実装する必要があるため、開発が複雑になることがあります。

4. 既存のスクリプト言語を組み込む:
LuaやPythonなどの既存のスクリプト言語をUnityに組み込むことができます。これにより、文字列をその言語のスクリプトとして実行できるようになります。これらの言語は、通常、C#のメソッドや関数を呼び出す機能を提供しています。ただし、これらの言語のランタイムをUnityに組み込むために、追加のライブラリやプラグインが必要になります。

注意: どの方法を選択するにせよ、文字列からスクリプトを実行することはセキュリティリスクがあるため、注意が必要です。特にユーザーからの入力を実行する場合には、十分な検証やサニタイズを行うことが重要です。

**筆者:**
できればUnityの設定に変更を加えず、C#のままで実行できる方法がほしいです。何かありますでしょうか。

**GPT-4:**
C#でスクリプトを実行する方法として、System.Reflectionを使って既存のC#スクリプト内のメソッドを実行する方法があります。ただし、これは完全なC#スクリプトの実行ではなく、既存のコード内の特定のメソッドを呼び出すことができる方法です。

例えば、以下のようなクラスとメソッドがあるとします。

~~~
public class MyExampleClass
{
    public void MyExampleMethod()
    {
        Debug.Log("Hello from MyExampleMethod!");
    }
}
~~~

System.Reflectionを使ってMyExampleMethodを実行するには、以下の手順に従います。

1. MyExampleClassのインスタンスを作成します。
2. MyExampleMethodに対応するMethodInfoオブジェクトを取得します。
3. MethodInfo.Invokeメソッドを使ってMyExampleMethodを実行します。

以下は具体的なコード例です。

~~~
using System.Reflection;
using UnityEngine;

public class ReflectionExample
{
    public void ExecuteMethod()
    {
        // クラスのインスタンスを作成
        MyExampleClass exampleInstance = new MyExampleClass();

        // MethodInfoオブジェクトを取得
        MethodInfo methodInfo = typeof(MyExampleClass).GetMethod("MyExampleMethod");

        // メソッドを実行
        methodInfo.Invoke(exampleInstance, null);
    }
}
~~~

この方法で既存のC#メソッドを文字列に基づいて実行できますが、完全に動的なC#コードの実行には対応していません。動的なC#コードを実行するには、前述の方法のいずれか（動的コンパイル、System.Reflection.Emit、独自のスクリプト言語、既存のスクリプト言語）を使用する必要があります。ただし、これらの方法はセキュリティリスクや互換性の問題を伴う場合がありますので、注意が必要です。

**筆者:**
例えば、以下のようなことがしたいのですが、可能でしょうか？

・GPT APIにやりたいことを伝えると、それを実現するスクリプトを返してくれる（必ずスクリプトを返してくれるものと仮定します）
・そのスクリプトをその場で実行して、やりたいことを実現する

もしできればコードを出力していただけますでしょうか。

**GPT-4:**
理想的には、GPT APIを使用して動的なC#コードを生成し、そのコードを実行することができます。ただし、実際にはセキュリティリスクや互換性の問題があります。このタイプの動的実行は、一般的には推奨されません。Unityでは、動的コンパイルやSystem.Reflection.Emitは限定的にサポートされており、特にIL2CPPビルドターゲットでは問題が発生する可能性があります。

もし、リスクを承知で実行したい場合は、CSharpCodeProviderを使用してC#コードを動的にコンパイルし、実行することができます。この方法は、.NET Frameworkに依存しており、Unityの.NET Standard 2.0や2.1の環境ではデフォルトでは使用できません。そのため、この方法は非推奨です。

以下に、CSharpCodeProviderを使用した例を示しますが、リスクを十分に理解した上で使用してください。

まず、以下のコードをDynamicCodeExecution.csという名前のスクリプトにコピーしてください。

~~~
using System;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.CSharp;
using UnityEngine;

public class DynamicCodeExecution : MonoBehaviour
{
    public string GptGeneratedCode = @"
        using UnityEngine;
        public class GeneratedClass
        {
            public static void GeneratedMethod()
            {
                Debug.Log(""Hello from the generated method!"");
            }
        }";

    private void Start()
    {
        ExecuteGeneratedCode(GptGeneratedCode);
    }

    private void ExecuteGeneratedCode(string code)
    {
        CSharpCodeProvider codeProvider = new CSharpCodeProvider();
        CompilerParameters parameters = new CompilerParameters
        {
            GenerateInMemory = true,
            GenerateExecutable = false,
            IncludeDebugInformation = false,
            ReferencedAssemblies = {
                "System.dll",
                "UnityEngine.dll",
                "mscorlib.dll"
            }
        };

        CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, code);

        if (results.Errors.HasErrors)
        {
            foreach (CompilerError error in results.Errors)
            {
                Debug.LogError($"Error (line {error.Line}): {error.ErrorText}");
            }
        }
        else
        {
            Assembly assembly = results.CompiledAssembly;
            Type generatedType = assembly.GetType("GeneratedClass");
            MethodInfo generatedMethod = generatedType.GetMethod("GeneratedMethod");

            if (generatedMethod != null)
            {
                generatedMethod.Invoke(null, null);
            }
            else
            {
                Debug.LogError("Couldn't find the generated method.");
            }
        }
    }
}
~~~

このスクリプトをUnityプロジェクト内の任意のGameObjectにアタッチし、実行してください。しかし、.NET Standard 2.0や2.1の環境ではCSharpCodeProviderがデフォルトで利用できないため、エラーが発生する可能性があります。

再度注意として、この方法は非推奨であり、セキュリティリスクや互

**筆者:**
例えば、返答の文字列を一旦スクリプトファイルとして出力して（Editor段階でのことなのでEditor拡張として出力することになるかと思います）、それを読み込んで実行する、というような形でやることはできますでしょうか？

**GPT-4:**
はい、そのような方法でスクリプトを実行することができます。具体的には、以下の手順で行います。

1. GPT APIから取得したコードを、プロジェクト内のスクリプトファイル（.cs）として保存します。
2. Unityエディタでスクリプトをコンパイルして、DLLとしてビルドされたアセンブリをロードします。
3. 反映されたアセンブリから、対象のクラスとメソッドをSystem.Reflectionを使って取得し、実行します。

以下に、Editor拡張としてスクリプトファイルを出力し、実行するサンプルコードを示します。

1. まず、GeneratedScriptExecutor.csというファイルを作成し、以下のコードをコピーしてください。

~~~
using System;
using System.Reflection;
using UnityEngine;

public class GeneratedScriptExecutor : MonoBehaviour
{
    public string AssemblyName = "Assembly-CSharp";
    public string ClassName = "GeneratedClass";
    public string MethodName = "GeneratedMethod";

    private void Start()
    {
        ExecuteGeneratedCode();
    }

    private void ExecuteGeneratedCode()
    {
        Assembly assembly = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.GetName().Name == AssemblyName)
            {
                assembly = asm;
                break;
            }
        }

        if (assembly != null)
        {
            Type generatedType = assembly.GetType(ClassName);
            MethodInfo generatedMethod = generatedType.GetMethod(MethodName);

            if (generatedMethod != null)
            {
                generatedMethod.Invoke(null, null);
            }
            else
            {
                Debug.LogError("Couldn't find the generated method.");
            }
        }
        else
        {
            Debug.LogError("Couldn't find the specified assembly.");
        }
    }
}
~~~

2. GPT APIから取得したコードを保存するためのEditor拡張を作成します。GptGeneratedScriptSaver.csというファイルを作成し、以下のコードをコピーしてください。

~~~
using UnityEditor;
using UnityEngine;
using System.IO;

public class GptGeneratedScriptSaver : EditorWindow
{
    private string generatedCode = "";
    private string scriptFileName = "GeneratedClass.cs";

    [MenuItem("Window/GptGeneratedScriptSaver")]
    public static void ShowWindow()
    {
        GetWindow<GptGeneratedScriptSaver>("GptGeneratedScriptSaver");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save GPT generated code as a script", EditorStyles.boldLabel);

        scriptFileName = EditorGUILayout.TextField("Script file name:", scriptFileName);

        GUILayout.Label("Generated code:");
        generatedCode = EditorGUILayout.TextArea(generatedCode, GUILayout.Height(200));

        if (GUILayout.Button("Save"))
        {
            SaveGeneratedCodeAsScript(generatedCode, scriptFileName);
        }
    }

    private void SaveGeneratedCodeAsScript(string code, string fileName)
    {
        string path = Path.Combine(Application.dataPath, fileName);

        File.WriteAllText(path, code);

        AssetDatabase.Refresh();
        Debug.Log("Generated script saved.");
    }
}
~~~

3. GPT APIから取得したコードをGptGeneratedScriptSaverウィンドウに貼り

**筆者:**
続けて下さい。

**GPT-4:**
3. GPT APIから取得したコードをGptGeneratedScriptSaverウィンドウに貼り付け、ファイル名を指定し、「Save」ボタンを押してスクリプトを保存してください。例えば、以下のようなコードが取得されたと仮定しましょう。

~~~
using UnityEngine;

public class GeneratedClass
{
    public static void GeneratedMethod()
    {
        Debug.Log("Hello from the generated method!");
    }
}
~~~

4. スクリプトが保存されたら、Unityエディタが自動的にコンパイルを行います。コンパイルが成功すれば、GeneratedClassがアセンブリに含まれます。
5. GeneratedScriptExecutorスクリプトをシーン内のGameObjectにアタッチして、対象のクラス名（GeneratedClass）、メソッド名（GeneratedMethod）を指定し、実行してください。GeneratedMethodが実行され、コンソールにメッセージが表示されます。

この方法では、GPT APIから取得したコードをスクリプトファイルとして保存し、コンパイルして実行することができます。ただし、この方法でもセキュリティリスクが存在することに注意してください。不正なコードが含まれている場合、システムに悪影響を与える可能性があります。

（ここでGPTが出力したコードを利用し、GPT呼び出し後直接返答をスクリプトに保存するコードを自力で実装しました）