const metaTable: any[string] = [];

// tslint:disable-next-line:max-line-length
// from: https://stackoverflow.com/questions/34950923/how-can-i-pass-additional-arguments-to-a-property-decorator-in-typescript
export function meta(o: any) {
  function actualDecorator(target: any, property: string): void {
    // https://stackoverflow.com/questions/46833911/typescript-proper-constructor-name-when-used-class-decorator-target-to-es5lib
    // minified TS->ES5 will lose original object.constructor.name
    // now emitting _className in TypeWriter ClassProjector.tst
    metaTable[target.constructor._className + "." + property] = o;
  }
  return actualDecorator;
}

export function metaLookup(className: string, propertyName: string, propKey?: string) {
  const obj: any = metaTable[className + "." + propertyName];
  return !propKey ? obj : obj && obj.hasOwnProperty(propKey) ? obj[propKey] : null;
}
