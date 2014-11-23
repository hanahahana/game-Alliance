using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace Alliance
{
  /// <summary>
  /// Provides a high level layer on top of the System.Reflection assembly.
  /// </summary>
  public static class ReflectionHelper
  {
    private static readonly BindingFlags __NonPublicBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

    /// <summary>
    /// Gets all of the subclass types of a specifed type found in the currently executing assemly.
    /// </summary>
    /// <param name="type">
    /// The class that each item in the list from the currently executing assembly should derive from.
    /// </param>
    /// <returns>A list of all of the subclasses of the specified type.</returns>
    public static List<Type> RetrieveAllSublcassTypes(Type type)
    {
      return RetrieveAllSublcassTypes(type, Assembly.GetExecutingAssembly());
    }

    /// <summary>
    /// Gets all of the subclass types of a specifed type found in a specified assembly.
    /// </summary>
    /// <param name="type">
    /// The class that each item in the list from the specified assembly should derive from.
    /// </param>
    /// <param name="assembly">
    /// The assembly to gather sub classes from and test if they derive from the specified type.
    /// </param>
    /// <returns>A list of all of the subclasses of the specified type.</returns>
    public static List<Type> RetrieveAllSublcassTypes(Type type, Assembly assembly)
    {
      return RetrieveAllSublcassTypes(type, new Assembly[] { assembly });
    }

    /// <summary>
    /// Gets all of the subclass types of a specified type found in a collection of assemblies.
    /// </summary>
    /// <param name="type">
    /// The class that each item in the list from the assemblies should derive from.
    /// </param>
    /// <param name="assemblies">
    /// The assemblies to gather sub classes from and test if they derive from the specified type.
    /// </param>
    /// <returns>A list of all of the subclasses of the specified type.</returns>
    public static List<Type> RetrieveAllSublcassTypes(Type type, Assembly[] assemblies)
    {
      List<Type> retval = new List<Type>();
      foreach (Assembly assembly in assemblies)
      {
        Type[] types = assembly.GetTypes();
        foreach (Type subType in types)
        {
          if (subType.IsSubclassOf(type))
          {
            retval.Add(subType);
          }
        }
      }

      return retval;
    }

    /// <summary>
    /// Calls the constructor of the specified type and returns the created
    /// object.
    /// </summary>
    /// <typeparam name="T">
    /// The strong type to return (this will be the corresponding default type if it doesn't match the type passed in.
    /// </typeparam>
    /// <param name="type">The type of object to create.</param>
    /// <param name="args">The arguments to pass to the constructor.</param>
    /// <returns>A strong typed object or the default value for the type if T doesn't match type.</returns>
    public static T CreateInstance<T>(Type type, params object[] args)
    {
      T retval = default(T);
      Type tType = typeof(T);

      if(type.IsSubclassOf(tType) || tType.IsSubclassOf(type) || type.Equals(tType))
      {
        retval = (T)Activator.CreateInstance(type, args);
      }
      return retval;
    }

    /// <summary>
    /// Creates an instance for each type in the enumerable object of types.
    /// </summary>
    /// <typeparam name="T">The of each value.</typeparam>
    /// <param name="types">The collection of types. Note, each type must be of type T.</param>
    /// <returns>A list of objects.</returns>
    public static List<T> CreateInstances<T>(IEnumerable<Type> types)
    {
      Type tType = typeof(T);
      List<T> retval = new List<T>();
      foreach (Type type in types)
      {
        if (type.Equals(tType) || type.IsSubclassOf(tType))
        {
          T item = (T)Activator.CreateInstance(type);
          retval.Add(item);
        }
      }
      return retval;
    }

    /// <summary>
    /// Returns the property info for a non-public (internal, protected, private) property.
    /// </summary>
    /// <param name="type">The type containing the property to obtain.</param>
    /// <param name="propertyName">The name of the property. This is exact case.</param>
    /// <returns>The matching property info, or null if none was found.</returns>
    public static PropertyInfo GetNonPublicPropertyInfo(Type type, string propertyName)
    {
      PropertyInfo retval = type.GetProperty(propertyName, __NonPublicBindingFlags);
      if (retval == null)
      {
        PropertyInfo[] properties = type.GetProperties(__NonPublicBindingFlags);
        for (int i = 0; i < properties.Length; ++i)
        {
          PropertyInfo info = properties[i];
          if (info.Name.Equals(propertyName))
          {
            retval = info;
            i = properties.Length;
          }
        }
      }

      return retval;
    }
  }
}
