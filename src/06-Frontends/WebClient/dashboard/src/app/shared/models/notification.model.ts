export enum NotificationLevel {
  error,
  info,
  success,
  warning
}

export type NotificationIconStyle = 'none' | 'success' | 'error' | 'warn' | 'info';

export class Notification {
  static __id = 0;

  private _id = 0;

  constructor(
    public level: NotificationLevel,
    public message: string,
  ) {
    this._id = Notification.__id++;
    this.date = new Date();
  }

  date: Date;

  get id() {
    return this._id;
  }

}
