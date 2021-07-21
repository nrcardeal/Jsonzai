# __Relatório__

## Introdução

Neste trabalho era pretendido desenvolver uma biblioteca que permitisse realizar o processamento de dados em formato JSON para instâncias de classes compatíveis. A biblioteca `Jsonzai` foi desenvolvida em liguagem C# com a máquina virtual .NET da empresa _Microsoft_.
***

## Estruturas de Dados

Para a realização desta biblioteca foi pensado eu usar a estrutura de dados `Dictionary` como estrutura de dados principais, esta mesma estrutura é organizada da seguinte forma, uma chave do tipo `Type` e um valor do tipo `Dictionary`, em que a chave é usada para associar a segunda estrutura de dados a uma classe, e a segunda estrutura de dados irá armazenar as propriedades da mesma, desta forma caso uma classe tenha propriedades do tipo referência que apresentem mais pripriedades, não é necessário um novo método de preenchimento das mesmas. A segunda estrutura de dados, que se encontra inserida dentro da primeira, apresenta uma `string` como chave, que contém o nome da propriedade na fonte JSON lida, e um valor que contêm uma instância com o método `SetValue()`, que preenche a propriedade, e o tipo da mesma.<br>
Para o preenchimento da estrutura de dados é utilizado o método `Cache()` o qual percorre todas as propriedades da classe recebida e verifica a existência de atributos na mesma, visto que são os mesmos que contém o nome costumizado na `string` JSON ou a classe onde a instância `setter` tem de ir buscar informação para o método `SetValue()` caso o utilizador não pretenda que seja usado o método incorporado na biblioteca `System.Reflection` da .NET. Na classe `JsonParsemit` essas verificações são realizadas durante a criação dos ficheiros DLL.
***

## Tipos Valor na criação da DLL

Quando a biblioteca está a tratar de um tipos valor para a criação de uma DLL é preciso emitir uma instrução IL de `Unbox_Any` em vez das instruções `Cast` ou `Castclass` usadas para tipos referência. Para tal são usados 3 métodos o método `RefereceTypeClassEmit()`, que é usado quando o tipo do objeto é do tipo referência, neste método é feita a instrução `Castclass`,  o método `ValueTypeClassEmit()`, usado em objetos do tipo valor onde é usada a instrução `Unbox_Any`, para poder ler o valor a colocar no objeto, esse valor é carregado para dentro de uma variavél primeiramente, depois a mesma é modificada para ficar com os valores corretos e posteriormente é ralizada a instrução `Box` que guarda no objeto o valor da variável. Existe ainda o método `PrepareValue()` que é responsável por preparar o valor para ser colocado no objeto quando necessário, no mesmo é verificado se a propriedade onde o valor irá ser colocado é do tipo valor ou referência, pois dependendo do mesmo será usada a instrução `Unbox_Any` ou `Cast`, respetivamente.
***

## Conversores via _Delegate_

Para a criação de conversores via _delegates_ foi criado um novo método estático, `AddConfiguration<T,W>()`, onde `T` é o tipo do objeto que contém as propriedades que irão ser preenchidas e `W` o tipo da propriedade do objeto onde se quer colocar um conversor, este método pode ser usado pelo cliente e recebe como parâmetros uma `string` que contém o nome da propriedade e um _delegate_ do tipo `Func<String,W>`. Neste método é chamado o método `Cache()` para preencher a estrutura de dados caso não estivesse já preenchida, de seguida procura-se no objeto uma propriedade que tenha o nome indicado no parâmetro recebido, é verificada se existe um atributo do tipo `JsonPropertyAtribute`, pois é este atributo que indica qual é o nome com que a propriedade se encontra apresentada na `string` JSON, e de seguida substitui na estrutura de dados, associada à classe do objeto, o valor associado à chave com o nome da propriedade na `string` fonte, por um valor com uma nova instância de `ISetter`, desta vez um `SetterConvertDelegate`, para o caso da classe `JsonParser`. Para o caso da classe `JsonParserEmit` essa substituição é feita por uma instância que tenha sido criada o código IL com a chamada ao _delegate_ através de um `call`, gerado no método `PrepareValue()`, que chama a propriedade `Method`, do tipo `MethodInfo` dos delegates.<br>
Existe uma otimização possivel de fazer a este código em que ao invés de ser chamado o método `Cache()` na primeira vez da chamada ao método `AddConfiguration<T,W>()` poderia ser feito um código semelhante ao que está a ser realizado no método `Cache()` mas com a verificação se a propriedade que está a ser alterada é a mesma da que foi passada como parâmetro. Essa otimização não foi realizada pois implicava repetição do código do método `Cache()` e o seu tempo de execução não seria otimizado o suficiente para ser recompensador a repetição do código. Encontra-se apresentado em baixo a imagem com os tempos de ambos os códigos e ainda o código alternativo para as duas classes.

### Tempos

### JsonParser

```ruby
public static void AddConfiguration<T, W>(string propName, Func<String, W> convert)
{
    PropertyInfo p = typeof(T).GetProperty(propName);
    if (!properties.ContainsKey(typeof(T)))
    {
        properties.Add(typeof(T), new Dictionary<string, ISetter>());
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            ISetter setter;
            if(p.Name == prop.Name)
                setter = new SetterConvertDelegate<W>(p, convert);
            else
            {
                JsonConvertAttribute conv = (JsonConvertAttribute)prop.GetCustomAttribute(typeof(JsonConvertAttribute));
                if (conv != null)
                    setter = new PropertySetterConvert(prop, conv.klass);
                else
                    setter = new PropertySetter(prop);
            }
            JsonPropertyAttribute attr =
                    (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
            if (attr != null)
                properties[typeof(T)].Add(attr.PropertyName, setter);
            else
                properties[typeof(T)].Add(prop.Name, setter);
        }
    }
    else
    {
        JsonPropertyAttribute attr = (JsonPropertyAttribute)p.GetCustomAttribute(typeof(JsonPropertyAttribute));
        if (attr != null)
            propName = attr.PropertyName;
        properties[typeof(T)].Remove(propName);
        properties[typeof(T)].Add(propName, new SetterConvertDelegate<W>(p, convert));
    }
}
```

### JsonParsemit

```ruby
public static void AddConfiguration<T, W>(string propName, Func<String, W> convert)
{
    PropertyInfo p = typeof(T).GetProperty(propName);
    if (!properties.ContainsKey(typeof(T)))
    {
        properties.Add(typeof(T), new Dictionary<string, ISetter>());
        foreach (PropertyInfo prop in typeof(T).GetProperties())
        {
            ISetter2 setter;
            if(p.Name == prop.Name)
                setter = (ISetter2)Activator.CreateInstance(BuildSetter(typeof(T), prop, convert));;
            else
                setter = (ISetter2)Activator.CreateInstance(BuildSetter(typeof(T), prop, null));
            JsonPropertyAttribute attr =
                    (JsonPropertyAttribute)prop.GetCustomAttribute(typeof(JsonPropertyAttribute));
            if (attr != null)
                properties[typeof(T)].Add(attr.PropertyName, setter);
            else
                properties[typeof(T)].Add(prop.Name, setter);
        }
    }
    else
    {
        JsonPropertyAttribute attr = (JsonPropertyAttribute)p.GetCustomAttribute(typeof(JsonPropertyAttribute));
        if (attr != null)
            propName = attr.PropertyName;
        properties[typeof(T)].Remove(propName);
        properties[typeof(T)].Add(propName, (ISetter2)Activator.CreateInstance(BuildSetter(typeof(T), prop, convert));
    }
}
```

***

## Conclusão

Este trabalho deu uma melhor preceção da relação entre programador e cliente, visto que o grupo teve de ir trocando entre um e outro para poder realizar o trabalho e testar o mesmo. Ajudou também a desenvolver as capacidades dos elementos do grupo na linguagem de C# e IL do ambiente virtual .NET, principalmente nas categorias de recursividade, emissão de código IL, tradução de código IL para C# e vice-versa e por fim na funcionalidade dos _delegates_.
