//from: https://mrpmorris.blogspot.com/2017/09/implementing-really-simple-elvis.html
//thank you !!

// https://stackoverflow.com/questions/47736473/how-to-define-global-function-in-typescript/47736563#47736563

abstract class Expression {
  //Brent: apparently rendering minified TS->JS drops 'unnecessary' semicolons!! (i.e. when published to a web site in "production" mode)
  private static readonly pathExtractor = new RegExp('return (.*?)[;|}]');

  public static pathAsArray<T>(name: (t: T) => any): string[] {
    const match = Expression.pathExtractor.exec(name + '');
    if (match == null) {
      throw new Error('The function does not contain a statement matching \'return variableName;\'');
    }
    return match[1].split('.').splice(1);
  }
}

export function isUndefinedOrNull(value: any): boolean {
  return (value === null) || (value === undefined);
}

declare global {
  function elvis<Source, Result=''>(instance: Source, member: (v: Source) => Result): any;
}

(window as any).elvis = <Source, Result=''>(instance: Source, member: (v: Source) => Result): any => {
  let path = Expression.pathAsArray(member);
  let result = instance as any;
  do {
    if (!isUndefinedOrNull(result) && (path.length > 0)) {
      const match = path[0].match(/\[([0-9]+)\]/); //see if we have array indexer
      if (match) result = result[path[0].replace(match[0], '')][Number(match[1])];
      else result = result[path[0]];
      path = path.slice(1);
    }
  } while (path.length > 0 && !isUndefinedOrNull(result));
  return result;
}

